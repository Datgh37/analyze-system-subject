import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ScrollView, Dimensions, ActivityIndicator } from 'react-native';
import { PieChart, BarChart } from 'react-native-chart-kit';
import { COLORS, SIZES } from '../constants/theme';
import apiClient from '../api/client';

const screenWidth = Dimensions.get('window').width;

/**
 * MÀN HÌNH BÁO CÁO BIỂU ĐỒ (REPORT SCREEN)
 * Kiến trúc & Kết nối API: Trịnh Thành Đạt (Lead)
 * Phụ trách hoàn thiện UI & Biểu đồ: Trần Trọng Đại (UI Specialist)
 * 
 * 👉 Nhiệm vụ Đại add nốt:
 * - Tinh chỉnh màu sắc, chú thích legend của PieChart & BarChart
 * - Bổ sung bảng tóm tắt dòng tiền ròng cuối kỳ
 */
export default function ReportScreen() {
  const [loading, setLoading] = useState(true);
  const [pieData, setPieData] = useState([]);
  const [barData, setBarData] = useState({
    labels: ['T5', 'T6', 'T7', 'T8', 'T9', 'T10'],
    datasets: [{ data: [0, 0, 0, 0, 0, 0] }]
  });

  useEffect(() => {
    fetchReportData();
  }, []);

  const fetchReportData = async () => {
    try {
      setLoading(true);
      // 1. Gọi API tỷ trọng chi tiêu (Pie Chart)
      const pieRes = await apiClient.get('/reports/category_chart.php?user_id=1&month=9&year=2026');
      if (pieRes && pieRes.data && pieRes.data.chart_data) {
        setPieData(pieRes.data.chart_data);
      }

      // 2. Gọi API đối chiếu thu chi (Bar Chart)
      const barRes = await apiClient.get('/reports/income_vs_expense_chart.php?user_id=1&months_count=6');
      if (barRes && barRes.data && barRes.data.chart_data) {
        setBarData({
          labels: barRes.data.chart_data.labels,
          datasets: [{ data: barRes.data.chart_data.expense_series }]
        });
      }
    } catch (err) {
      console.log('Lỗi tải báo cáo biểu đồ:', err.message);
    } finally {
      setLoading(false);
    }
  };

  const chartConfig = {
    backgroundGradientFrom: COLORS.card,
    backgroundGradientTo: COLORS.card,
    color: (opacity = 1) => `rgba(46, 125, 50, ${opacity})`,
    labelColor: (opacity = 1) => `rgba(33, 33, 33, ${opacity})`,
    strokeWidth: 2,
    barPercentage: 0.6,
    decimalPlaces: 0
  };

  return (
    <ScrollView style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.headerTitle}>Báo Cáo Tài Chính & Biểu Đồ</Text>
      </View>

      {loading ? (
        <ActivityIndicator size="large" color={COLORS.primary} style={{ marginTop: 40 }} />
      ) : (
        <>
          {/* 1. Biểu đồ tròn cơ cấu chi tiêu */}
          <View style={styles.chartCard}>
            <Text style={styles.cardTitle}>Tỷ Trọng Chi Tiêu Tháng 09/2026</Text>
            {pieData.length > 0 ? (
              <PieChart
                data={pieData}
                width={screenWidth - SIZES.md * 2}
                height={200}
                chartConfig={chartConfig}
                accessor="amount"
                backgroundColor="transparent"
                paddingLeft="15"
                absolute
              />
            ) : (
              <Text style={styles.emptyText}>Chưa có dữ liệu chi tiêu tháng này.</Text>
            )}
          </View>

          {/* 2. Biểu đồ cột chi tiêu qua các tháng */}
          <View style={styles.chartCard}>
            <Text style={styles.cardTitle}>Biến Động Chi Tiêu 6 Tháng Gần Nhất</Text>
            <BarChart
              data={barData}
              width={screenWidth - SIZES.md * 2}
              height={220}
              yAxisLabel=""
              yAxisSuffix=""
              chartConfig={chartConfig}
              verticalLabelRotation={0}
              fromZero
            />
          </View>
        </>
      )}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background
  },
  header: {
    padding: SIZES.md,
    backgroundColor: COLORS.card,
    borderBottomWidth: 1,
    borderBottomColor: COLORS.border
  },
  headerTitle: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.text
  },
  chartCard: {
    backgroundColor: COLORS.card,
    marginHorizontal: SIZES.md,
    marginTop: SIZES.md,
    borderRadius: SIZES.radiusMd,
    padding: SIZES.md,
    elevation: 2,
    alignItems: 'center'
  },
  cardTitle: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.text,
    marginBottom: SIZES.sm,
    alignSelf: 'flex-start'
  },
  emptyText: {
    paddingVertical: SIZES.lg,
    color: COLORS.textSecondary,
    fontSize: SIZES.fontMd
  }
});
