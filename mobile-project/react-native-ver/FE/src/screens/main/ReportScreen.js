import React, { useState, useEffect, useCallback } from 'react';
import { View, Text, StyleSheet, ActivityIndicator, RefreshControl, ScrollView } from 'react-native';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES } from '../../constants/theme';

/**
 * ==============================================================================
 * MÀN HÌNH: BÁO CÁO TÀI CHÍNH & BIỂU ĐỒ (REPORT SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Đã tích hợp sẵn 100% bên dưới
 * ==============================================================================
 * 
 * 📌 DỮ LIỆU ĐÃ TÍCH HỢP TỪ BACKEND TRONG STATE SẴN SÀNG CHO BIỂU ĐỒ:
 * 1. `summaryData`:
 *    - `summaryData.total_income`: Tổng thu nhập tháng (float)
 *    - `summaryData.total_expense`: Tổng chi tiêu tháng (float)
 *    - `summaryData.net_balance`: Tiết kiệm ròng (Thu - Chi)
 *    - `summaryData.savings_rate`: Tỷ lệ tiết kiệm (%)
 * 
 * 2. `pieChartData`:
 *    - Dữ liệu chuẩn tương thích thẳng với `<PieChart>` của react-native-chart-kit:
 *      [{ name, population, color, icon_name, percentage, legendFontColor, legendFontSize }, ...]
 * 
 * 3. `barChartData`:
 *    - Dữ liệu đối chiếu thu chi 6 tháng tương thích `<BarChart>`:
 *      { labels: ['T4', 'T5', 'T6', ...], income_series: [...], expense_series: [...] }
 * 
 * 4. Hàm:
 *    - `formatCurrency(amount)`: Định dạng tiền VND
 * ==============================================================================
 */

export default function ReportScreen() {
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [summaryData, setSummaryData] = useState(null);
  const [pieChartData, setPieChartData] = useState([]);
  const [barChartData, setBarChartData] = useState(null);

  const fetchReports = async () => {
    try {
      const [sumRes, pieRes, barRes] = await Promise.all([
        api.getMonthlySummary(),
        api.getCategoryChart(),
        api.getIncomeVsExpenseChart(6)
      ]);

      if (sumRes.data) setSummaryData(sumRes.data);
      if (pieRes.data?.chart_data) setPieChartData(pieRes.data.chart_data);
      if (barRes.data?.chart_data) setBarChartData(barRes.data.chart_data);
    } catch (error) {
      console.warn('[ReportScreen] Lỗi nạp báo cáo:', error.message);
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

  const formatCurrency = (amt) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amt || 0);
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color={COLORS.primary} />
      </View>
    );
  }

  return (
    <ScrollView
      style={styles.container}
      contentContainerStyle={styles.contentContainer}
      refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={[COLORS.primary]} />}
    >
      <View style={styles.placeholderBox}>
        <Text style={styles.title}>Màn hình Báo Cáo & Biểu Đồ</Text>
        <Text style={styles.subtitle}>[Khu vực Trần Trọng Đại tự do thiết kế giao diện UI]</Text>

        <View style={styles.dataPreviewBox}>
          <Text style={styles.dataPreviewTitle}>Dữ liệu báo cáo sẵn sàng kết nối Biểu đồ:</Text>
          <Text style={styles.dataPreviewText}>• Tổng Thu: +{formatCurrency(summaryData?.total_income)}</Text>
          <Text style={styles.dataPreviewText}>• Tổng Chi: -{formatCurrency(summaryData?.total_expense)}</Text>
          <Text style={styles.dataPreviewText}>• Thặng dư ròng: {formatCurrency(summaryData?.net_balance)} (Tích lũy {summaryData?.savings_rate}%)</Text>
          <Text style={styles.dataPreviewText}>• Số lát PieChart danh mục: {pieChartData.length} danh mục</Text>
          <Text style={styles.dataPreviewText}>• Số cột BarChart đối chiếu: {barChartData?.labels?.length || 0} tháng</Text>
        </View>

        <Text style={styles.hint}>
          Đại xem hướng dẫn trong file code: Nạp `pieChartData` vào PieChart và `barChartData` vào BarChart (react-native-chart-kit).
        </Text>
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
    padding: SPACING.lg,
    flexGrow: 1,
    justifyContent: 'center'
  },
  centerContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center'
  },
  placeholderBox: {
    backgroundColor: COLORS.surface,
    padding: SPACING.xl,
    borderRadius: SIZES.radiusLg,
    borderWidth: 1,
    borderColor: COLORS.border,
    alignItems: 'center'
  },
  title: {
    fontSize: SIZES.fontXl,
    fontWeight: 'bold',
    color: COLORS.primaryDark,
    marginBottom: SPACING.xs
  },
  subtitle: {
    fontSize: SIZES.fontSm,
    color: COLORS.primary,
    fontWeight: '600',
    textAlign: 'center',
    marginBottom: SPACING.lg
  },
  dataPreviewBox: {
    backgroundColor: COLORS.background,
    padding: SPACING.md,
    borderRadius: SIZES.radiusMd,
    width: '100%',
    marginBottom: SPACING.md
  },
  dataPreviewTitle: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold',
    color: COLORS.primaryDark,
    marginBottom: SPACING.xs
  },
  dataPreviewText: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    marginBottom: 2
  },
  hint: {
    fontSize: SIZES.fontXs,
    color: COLORS.textMuted,
    textAlign: 'center'
  }
});
