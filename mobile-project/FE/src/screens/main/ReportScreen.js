import React, { useState, useEffect, useCallback } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ActivityIndicator,
  RefreshControl,
  ScrollView,
  Dimensions
} from 'react-native';
import { PieChart, BarChart } from 'react-native-chart-kit';
import { Ionicons } from '@expo/vector-icons';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES, SHADOWS } from '../../constants/theme';

const screenWidth = Dimensions.get('window').width;

/**
 * ==============================================================================
 * MÀN HÌNH: BÁO CÁO TÀI CHÍNH & BIỂU ĐỒ (REPORT SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Trịnh Thành Đạt (Lead Architect)
 * ==============================================================================
 */
export default function ReportScreen() {
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  // Dữ liệu tổng hợp dòng tiền
  const [summaryData, setSummaryData] = useState({
    total_income: 15000000,
    total_expense: 8500000,
    net_balance: 6500000,
    savings_rate: 43
  });

  // Dữ liệu biểu đồ tròn tỷ trọng chi tiêu (PieChart)
  const [pieChartData, setPieChartData] = useState([
    {
      name: 'Ăn uống',
      population: 3500000,
      color: '#FF6B6B',
      legendFontColor: '#1E293B',
      legendFontSize: 12
    },
    {
      name: 'Nhà ở & Tiện ích',
      population: 2500000,
      color: '#4D96FF',
      legendFontColor: '#1E293B',
      legendFontSize: 12
    },
    {
      name: 'Mua sắm',
      population: 1200000,
      color: '#4ECDC4',
      legendFontColor: '#1E293B',
      legendFontSize: 12
    },
    {
      name: 'Đi lại',
      population: 800000,
      color: '#FFE66D',
      legendFontColor: '#1E293B',
      legendFontSize: 12
    },
    {
      name: 'Giải trí & Khác',
      population: 500000,
      color: '#9B5DE5',
      legendFontColor: '#1E293B',
      legendFontSize: 12
    }
  ]);

  // Dữ liệu biểu đồ cột đối chiếu 6 tháng (BarChart)
  const [barChartData, setBarChartData] = useState({
    labels: ['T5', 'T6', 'T7', 'T8', 'T9', 'T10'],
    datasets: [
      {
        data: [7.2, 8.5, 9.1, 7.8, 8.5, 8.5]
      }
    ]
  });

  const fetchReports = async () => {
    try {
      const [sumRes, pieRes, barRes] = await Promise.allSettled([
        api.getMonthlySummary(),
        api.getCategoryChart(),
        api.getIncomeVsExpenseChart(6)
      ]);

      if (sumRes.status === 'fulfilled' && sumRes.value?.data) {
        setSummaryData(sumRes.value.data);
      }

      if (pieRes.status === 'fulfilled' && pieRes.value?.data) {
        const rawPie = pieRes.value.data.chart_data || pieRes.value.data;
        if (Array.isArray(rawPie) && rawPie.length > 0) {
          const formatted = rawPie.map((item, idx) => ({
            name: item.name || item.category_name,
            population: parseFloat(item.population || item.amount || item.total_amount || 0),
            color: item.color || item.color_code || ['#FF6B6B', '#4D96FF', '#4ECDC4', '#FFE66D', '#9B5DE5'][idx % 5],
            legendFontColor: '#1E293B',
            legendFontSize: 12
          }));
          setPieChartData(formatted);
        }
      }

      if (barRes.status === 'fulfilled' && barRes.value?.data) {
        const rawBar = barRes.value.data.chart_data || barRes.value.data;
        if (rawBar?.labels && rawBar?.expense_series) {
          setBarChartData({
            labels: rawBar.labels,
            datasets: [
              {
                data: rawBar.expense_series.map((val) => parseFloat((val / 1000000).toFixed(1)))
              }
            ]
          });
        }
      }
    } catch (error) {
      console.warn('[ReportScreen] Lỗi nạp báo cáo, dùng mock fallback:', error.message);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    fetchReports();
  }, []);

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    fetchReports();
  }, []);

  const chartConfig = {
    backgroundGradientFrom: COLORS.surface,
    backgroundGradientTo: COLORS.surface,
    color: (opacity = 1) => `rgba(43, 76, 126, ${opacity})`,
    labelColor: (opacity = 1) => `rgba(100, 116, 139, ${opacity})`,
    barPercentage: 0.6,
    decimalPlaces: 1,
    fillShadowGradient: COLORS.primary,
    fillShadowGradientOpacity: 1
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color={COLORS.primary} />
        <Text style={styles.loadingText}>Đang tính toán biểu đồ tài chính...</Text>
      </View>
    );
  }

  const totalExpense = summaryData.total_expense || 0;
  const totalIncome = summaryData.total_income || 0;
  const netBalance = summaryData.net_balance ?? (totalIncome - totalExpense);
  const savingsRate = summaryData.savings_rate ?? (totalIncome > 0 ? Math.round((netBalance / totalIncome) * 100) : 0);

  return (
    <ScrollView
      style={styles.container}
      contentContainerStyle={styles.contentContainer}
      refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={[COLORS.primary]} />}
    >
      {/* 1. Thẻ Tóm Tắt Dòng Tiền Ròng & Tỷ Lệ Tiết Kiệm */}
      <View style={styles.summaryCard}>
        <Text style={styles.cardHeaderTitle}>TỔNG KẾT TÀI CHÍNH THÁNG</Text>

        <View style={styles.summaryRow}>
          <View style={styles.summaryCol}>
            <View style={styles.labelRow}>
              <Ionicons name="arrow-down-circle" size={16} color={COLORS.income} style={{ marginRight: 4 }} />
              <Text style={styles.incomeLabel}>Tổng Thu</Text>
            </View>
            <Text style={styles.incomeAmount}>+{totalIncome.toLocaleString('vi-VN')} đ</Text>
          </View>

          <View style={styles.summaryDivider} />

          <View style={styles.summaryCol}>
            <View style={styles.labelRow}>
              <Ionicons name="arrow-up-circle" size={16} color={COLORS.expense} style={{ marginRight: 4 }} />
              <Text style={styles.expenseLabel}>Tổng Chi</Text>
            </View>
            <Text style={styles.expenseAmount}>-{totalExpense.toLocaleString('vi-VN')} đ</Text>
          </View>
        </View>

        <View style={styles.netBalanceRow}>
          <View>
            <Text style={styles.netLabel}>Tiết kiệm ròng:</Text>
            <Text style={[styles.netAmount, { color: netBalance >= 0 ? COLORS.income : COLORS.danger }]}>
              {netBalance >= 0 ? '+' : ''}{netBalance.toLocaleString('vi-VN')} đ
            </Text>
          </View>
          <View style={styles.savingsBadge}>
            <Text style={styles.savingsBadgeText}>Tích lũy: {savingsRate}%</Text>
          </View>
        </View>
      </View>

      {/* 2. Biểu Đồ Tròn Tỷ Trọng Chi Tiêu (PieChart) */}
      <View style={styles.chartCard}>
        <View style={styles.chartHeaderRow}>
          <Ionicons name="pie-chart" size={20} color={COLORS.primary} style={{ marginRight: 6 }} />
          <Text style={styles.chartTitle}>Cơ Cấu Chi Tiêu Theo Danh Mục</Text>
        </View>

        <PieChart
          data={pieChartData}
          width={screenWidth - SPACING.md * 2 - 32}
          height={200}
          chartConfig={chartConfig}
          accessor="population"
          backgroundColor="transparent"
          paddingLeft="10"
          absolute
        />
      </View>

      {/* 3. Biểu Đồ Cột Đối Chiếu 6 Tháng (BarChart) */}
      <View style={styles.chartCard}>
        <View style={styles.chartHeaderRow}>
          <Ionicons name="bar-chart" size={20} color={COLORS.primary} style={{ marginRight: 6 }} />
          <Text style={styles.chartTitle}>Xu Hướng Chi Tiêu 6 Tháng Gần Nhất</Text>
        </View>
        <Text style={styles.unitHint}>(Đơn vị: Triệu VNĐ)</Text>

        <BarChart
          data={barChartData}
          width={screenWidth - SPACING.md * 2 - 32}
          height={220}
          yAxisLabel=""
          yAxisSuffix="Tr"
          chartConfig={chartConfig}
          verticalLabelRotation={0}
          fromZero
          style={styles.barChartStyle}
        />
      </View>

      {/* 4. Danh Sách Phân Loại Tỷ Trọng Chi Tiết */}
      <View style={styles.detailCard}>
        <Text style={styles.detailCardTitle}>Bảng Chi Tiết Tỷ Trọng Danh Mục</Text>

        {pieChartData.map((item, idx) => {
          const pct = totalExpense > 0 ? Math.round((item.population / totalExpense) * 100) : 0;
          return (
            <View key={idx} style={styles.detailRow}>
              <View style={styles.detailLeft}>
                <View style={[styles.detailDot, { backgroundColor: item.color }]} />
                <Text style={styles.detailName}>{item.name}</Text>
              </View>
              <View style={styles.detailRight}>
                <Text style={styles.detailAmount}>{item.population.toLocaleString('vi-VN')} đ</Text>
                <Text style={styles.detailPct}>({pct}%)</Text>
              </View>
            </View>
          );
        })}
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background
  },
  contentContainer: {
    padding: SPACING.md,
    paddingBottom: SPACING.xxl
  },
  centerContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: COLORS.background
  },
  loadingText: {
    marginTop: SPACING.sm,
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  summaryCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusLg,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    ...SHADOWS.card
  },
  cardHeaderTitle: {
    fontSize: SIZES.fontXs,
    fontWeight: 'bold',
    color: COLORS.textSecondary,
    letterSpacing: 0.5,
    marginBottom: SPACING.sm
  },
  summaryRow: {
    flexDirection: 'row',
    paddingBottom: SPACING.sm,
    borderBottomWidth: 1,
    borderBottomColor: COLORS.borderLight
  },
  summaryCol: {
    flex: 1
  },
  labelRow: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  incomeLabel: {
    fontSize: SIZES.fontSm,
    color: COLORS.income,
    fontWeight: '600'
  },
  incomeAmount: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.income,
    marginTop: 2
  },
  expenseLabel: {
    fontSize: SIZES.fontSm,
    color: COLORS.expense,
    fontWeight: '600'
  },
  expenseAmount: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.expense,
    marginTop: 2
  },
  summaryDivider: {
    width: 1,
    backgroundColor: COLORS.borderLight,
    marginHorizontal: SPACING.sm
  },
  netBalanceRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginTop: SPACING.sm
  },
  netLabel: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary
  },
  netAmount: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    marginTop: 2
  },
  savingsBadge: {
    backgroundColor: '#E0F2FE',
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.xs + 2,
    borderRadius: SIZES.radiusFull
  },
  savingsBadgeText: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold',
    color: '#0284C7'
  },
  chartCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusLg,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    alignItems: 'center',
    ...SHADOWS.small
  },
  chartHeaderRow: {
    flexDirection: 'row',
    alignItems: 'center',
    alignSelf: 'flex-start',
    marginBottom: 4
  },
  chartTitle: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.textPrimary
  },
  unitHint: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    alignSelf: 'flex-start',
    marginBottom: SPACING.sm
  },
  barChartStyle: {
    marginVertical: 4,
    borderRadius: SIZES.radiusMd
  },
  detailCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusLg,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    ...SHADOWS.small
  },
  detailCardTitle: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.textPrimary,
    marginBottom: SPACING.sm
  },
  detailRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: SPACING.xs + 2,
    borderBottomWidth: 1,
    borderBottomColor: COLORS.borderLight
  },
  detailLeft: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  detailDot: {
    width: 10,
    height: 10,
    borderRadius: 5,
    marginRight: 8
  },
  detailName: {
    fontSize: SIZES.fontSm,
    color: COLORS.textPrimary
  },
  detailRight: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  detailAmount: {
    fontSize: SIZES.fontSm,
    fontWeight: '600',
    color: COLORS.textPrimary,
    marginRight: 4
  },
  detailPct: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary
  }
});
