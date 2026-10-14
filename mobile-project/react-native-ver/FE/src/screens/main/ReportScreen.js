import React, { useState, useEffect, useCallback } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ActivityIndicator,
  RefreshControl,
  ScrollView,
  Dimensions,
  TouchableOpacity
} from 'react-native';
import { PieChart, BarChart } from 'react-native-chart-kit';
import { Ionicons } from '@expo/vector-icons';
import { api } from '../../api/endpoints';
import { reportStyles as styles, COLORS, SPACING } from '../../constants/appStyles';

const screenWidth = Dimensions.get('window').width;

/**
 * ==============================================================================
 * MÀN HÌNH: BÁO CÁO TÀI CHÍNH & BIỂU ĐỒ (REPORT SCREEN)
 * Quản lý Chi tiêu & Hoạch định Tài chính Cá nhân Thông minh
 * ==============================================================================
 */
export default function ReportScreen({ navigation }) {
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [errorMsg, setErrorMsg] = useState(null);

  // 100% Dữ liệu thực tế từ CSDL MySQL (Khởi tạo rỗng, không hardcode mock data)
  const [summaryData, setSummaryData] = useState({
    total_income: 0,
    total_expense: 0,
    net_balance: 0,
    savings_rate: 0
  });

  const [pieChartData, setPieChartData] = useState([]);

  // Chế độ xem biểu đồ 6 tháng: 'EXPENSE' (Chi tiêu) hoặc 'INCOME' (Thu nhập)
  const [chartMode, setChartMode] = useState('EXPENSE');

  // Dữ liệu 6 tháng đối chiếu từ CSDL
  const [sixMonthData, setSixMonthData] = useState({
    labels: [],
    expense_series: [],
    income_series: [],
    monthly_details: []
  });

  const fetchReports = async () => {
    try {
      setErrorMsg(null);
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
        } else {
          setPieChartData([]);
        }
      }

      if (barRes.status === 'fulfilled' && barRes.value?.data) {
        const rawBar = barRes.value.data.chart_data || barRes.value.data;
        const details = barRes.value.data.monthly_details || [];
        if (rawBar?.labels) {
          setSixMonthData({
            labels: rawBar.labels,
            expense_series: (rawBar.expense_series || []).map((val) => parseFloat((val / 1000000).toFixed(1))),
            income_series: (rawBar.income_series || []).map((val) => parseFloat((val / 1000000).toFixed(1))),
            monthly_details: details
          });
        }
      }
    } catch (error) {
      console.warn('[ReportScreen] Lỗi nạp báo cáo từ Backend:', error.message);
      setErrorMsg(error.message || 'Không thể tải biểu đồ báo cáo từ máy chủ.');
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

  const chartThemeColor = chartMode === 'EXPENSE' ? COLORS.primary : COLORS.income;

  const chartConfig = {
    backgroundGradientFrom: COLORS.surface,
    backgroundGradientTo: COLORS.surface,
    color: (opacity = 1) => chartMode === 'EXPENSE' 
      ? `rgba(43, 76, 126, ${opacity})` 
      : `rgba(16, 185, 129, ${opacity})`,
    labelColor: (opacity = 1) => `rgba(100, 116, 139, ${opacity})`,
    barPercentage: 0.55,
    decimalPlaces: 1,
    fillShadowGradient: chartThemeColor,
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

  const hasPieData = pieChartData && pieChartData.length > 0 && pieChartData.some((item) => item.population > 0);

  // Dữ liệu hiển thị biểu đồ cột theo chế độ đã chọn (Chi tiêu / Thu nhập)
  const currentSeries = chartMode === 'EXPENSE' ? sixMonthData.expense_series : sixMonthData.income_series;
  const hasBarData = currentSeries && currentSeries.length > 0 && currentSeries.some((val) => val > 0);
  const barChartDisplayData = {
    labels: sixMonthData.labels,
    datasets: [
      {
        data: hasBarData ? currentSeries : [0, 0, 0, 0, 0, 0]
      }
    ]
  };

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

        {hasPieData ? (
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
        ) : (
          <View style={styles.chartEmptyWrap}>
            <Ionicons name="pie-chart-outline" size={44} color={COLORS.textMuted} />
            <Text style={styles.chartEmptyText}>Chưa có phát sinh chi tiêu trong kỳ này</Text>
            <TouchableOpacity
              style={styles.emptyActionBtn}
              onPress={() => navigation?.navigate('AddTransaction')}
            >
              <Text style={styles.emptyActionBtnText}>+ Thêm khoản chi</Text>
            </TouchableOpacity>
          </View>
        )}
      </View>

      {/* 3. Biểu Đồ Cột Đối Chiếu 6 Tháng (BarChart) - Có Chuyển Đổi Thu / Chi */}
      <View style={styles.chartCard}>
        <View style={styles.chartHeaderRow}>
          <Ionicons name="bar-chart" size={20} color={chartThemeColor} style={{ marginRight: 6 }} />
          <Text style={styles.chartTitle}>
            {chartMode === 'EXPENSE' ? 'Xu Hướng Chi Tiêu 6 Tháng' : 'Xu Hướng Thu Nhập 6 Tháng'}
          </Text>
        </View>
        <Text style={styles.unitHint}>(Đơn vị: Triệu VNĐ • Bấm chuyển đổi bên dưới)</Text>

        {/* Nút chuyển đổi tương tác Chi tiêu / Thu nhập */}
        <View style={styles.chartToggleRow}>
          <TouchableOpacity
            style={[styles.chartToggleBtn, chartMode === 'EXPENSE' && styles.chartToggleBtnActive]}
            onPress={() => setChartMode('EXPENSE')}
            activeOpacity={0.7}
          >
            <Ionicons
              name="arrow-up-circle-outline"
              size={15}
              color={chartMode === 'EXPENSE' ? '#FFFFFF' : COLORS.expense}
              style={{ marginRight: 4 }}
            />
            <Text style={[styles.chartToggleText, chartMode === 'EXPENSE' && styles.chartToggleTextActive]}>
              Chi tiêu (Expense)
            </Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={[styles.chartToggleBtn, chartMode === 'INCOME' && styles.chartToggleBtnActiveIncome]}
            onPress={() => setChartMode('INCOME')}
            activeOpacity={0.7}
          >
            <Ionicons
              name="arrow-down-circle-outline"
              size={15}
              color={chartMode === 'INCOME' ? '#FFFFFF' : COLORS.income}
              style={{ marginRight: 4 }}
            />
            <Text style={[styles.chartToggleText, chartMode === 'INCOME' && styles.chartToggleTextActive]}>
              Thu nhập (Income)
            </Text>
          </TouchableOpacity>
        </View>

        {hasBarData ? (
          <BarChart
            data={barChartDisplayData}
            width={screenWidth - SPACING.md * 2 - 32}
            height={220}
            yAxisLabel=""
            yAxisSuffix="Tr"
            chartConfig={chartConfig}
            verticalLabelRotation={0}
            fromZero
            style={styles.barChartStyle}
          />
        ) : (
          <View style={styles.chartEmptyWrap}>
            <Ionicons name="bar-chart-outline" size={44} color={COLORS.textMuted} />
            <Text style={styles.chartEmptyText}>Chưa đủ lịch sử 6 tháng để hiển thị xu hướng</Text>
          </View>
        )}

        {/* Bảng chi tiết đối chiếu dòng tiền 6 tháng gần nhất */}
        {sixMonthData.monthly_details && sixMonthData.monthly_details.length > 0 && (
          <View style={styles.historyTable}>
            <View style={[styles.historyTableRow, { borderBottomWidth: 1.5, borderBottomColor: COLORS.border }]}>
              <Text style={[styles.historyTableColMonth, { fontWeight: 'bold' }]}>Tháng</Text>
              <Text style={[styles.historyTableColIncome, { fontWeight: 'bold' }]}>Tổng Thu</Text>
              <Text style={[styles.historyTableColExpense, { fontWeight: 'bold' }]}>Tổng Chi</Text>
              <Text style={[styles.historyTableColNet, { fontWeight: 'bold' }]}>Ròng (+/-)</Text>
            </View>

            {sixMonthData.monthly_details.map((mItem, idx) => {
              const incMil = (mItem.total_income / 1000000).toFixed(1);
              const expMil = (mItem.total_expense / 1000000).toFixed(1);
              const netMil = (mItem.net_savings / 1000000).toFixed(1);
              const isPositive = mItem.net_savings >= 0;

              return (
                <View key={idx} style={styles.historyTableRow}>
                  <Text style={styles.historyTableColMonth}>{mItem.full_label || `Thg ${mItem.month}`}</Text>
                  <Text style={styles.historyTableColIncome}>+{incMil}Tr</Text>
                  <Text style={styles.historyTableColExpense}>-{expMil}Tr</Text>
                  <Text style={[styles.historyTableColNet, { color: isPositive ? COLORS.income : COLORS.danger }]}>
                    {isPositive ? '+' : ''}{netMil}Tr
                  </Text>
                </View>
              );
            })}
          </View>
        )}
      </View>

      {/* 4. Danh Sách Phân Loại Tỷ Trọng Chi Tiết */}
      {hasPieData && (
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
      )}
    </ScrollView>
  );
}
