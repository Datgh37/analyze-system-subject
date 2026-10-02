import React, { useState, useEffect, useCallback } from 'react';
import { View, Text, StyleSheet, ActivityIndicator, RefreshControl, ScrollView, Alert } from 'react-native';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES } from '../../constants/theme';

/**
 * ==============================================================================
 * MÀN HÌNH: HẠN MỨC NGÂN SÁCH (BUDGET SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Đã tích hợp sẵn 100% bên dưới
 * ==============================================================================
 * 
 * 📌 CÁC DỮ LIỆU ĐÃ TÍCH HỢP TỪ BACKEND TRONG STATE:
 * 1. `budgetData`:
 *    - `budgetData.overall_budget`:
 *      + `limit_amount`: Hạn mức tối đa cả tháng
 *      + `spent_amount`: Số tiền thực tế đã chi trong tháng
 *      + `remaining_amount`: Số tiền còn lại
 *      + `percentage`: Tỷ lệ % đã tiêu
 *      + `status_level`: Mức cảnh báo ('NORMAL' | 'WARNING' | 'DANGER')
 *      + `status_text`: Chuỗi thông báo cảnh báo thông minh
 *      + `days_remaining`: Số ngày còn lại trong tháng
 *      + `daily_safe_spending`: Số tiền chi tiêu an toàn mỗi ngày
 *    - `budgetData.category_budgets`: Mảng hạn mức theo từng danh mục
 * 
 * 2. Hàm xử lý (Handler):
 *    - `handleSaveBudget(limit)`: Gọi API thiết lập/cập nhật hạn mức tháng
 *    - `onRefresh()`: Kéo xuống cập nhật số liệu mới nhất
 * ==============================================================================
 */

export default function BudgetScreen() {
  const [budgetData, setBudgetData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const fetchBudgetStatus = async () => {
    try {
      const res = await api.getBudgetStatus();
      if (res.data) {
        setBudgetData(res.data);
      }
    } catch (error) {
      console.warn('[BudgetScreen] Lỗi nạp ngân sách:', error.message);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    fetchBudgetStatus();
  }, []);

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    fetchBudgetStatus();
  }, []);

  const handleSaveBudget = async (newLimitAmount) => {
    try {
      await api.setBudget({
        categoryId: null,
        amountLimit: newLimitAmount,
        month: budgetData?.month || new Date().getMonth() + 1,
        year: budgetData?.year || new Date().getFullYear()
      });
      Alert.alert('Thành công', 'Đã lưu hạn mức ngân sách tháng!');
      fetchBudgetStatus();
    } catch (error) {
      Alert.alert('Lỗi', error.message);
    }
  };

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

  const overall = budgetData?.overall_budget;

  return (
    <ScrollView
      style={styles.container}
      contentContainerStyle={styles.contentContainer}
      refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={[COLORS.primary]} />}
    >
      <View style={styles.placeholderBox}>
        <Text style={styles.title}>Màn hình Quản Lý Ngân Sách</Text>
        <Text style={styles.subtitle}>[Khu vực Trần Trọng Đại tự do thiết kế giao diện UI]</Text>

        <View style={styles.dataPreviewBox}>
          <Text style={styles.dataPreviewTitle}>Dữ liệu tiến độ ngân sách tháng {budgetData?.month}/{budgetData?.year}:</Text>
          <Text style={styles.dataPreviewText}>• Hạn mức đặt ra: {formatCurrency(overall?.limit_amount)}</Text>
          <Text style={styles.dataPreviewText}>• Thực tế đã chi: {formatCurrency(overall?.spent_amount)} ({overall?.percentage}%)</Text>
          <Text style={styles.dataPreviewText}>• Còn lại: {formatCurrency(overall?.remaining_amount)}</Text>
          <Text style={styles.dataPreviewText}>• Mức cảnh báo: {overall?.status_level} ({overall?.status_text})</Text>
          <Text style={styles.dataPreviewText}>• Tốc độ chi an toàn: ~{formatCurrency(overall?.daily_safe_spending)}/ngày (còn {overall?.days_remaining} ngày)</Text>
          <Text style={styles.dataPreviewText}>• Ngân sách danh mục: {budgetData?.category_budgets?.length || 0} danh mục</Text>
        </View>

        <Text style={styles.hint}>
          Đại xem hướng dẫn trong code: Dùng các trường trong `budgetData` để vẽ thanh Progress Bar và Card cảnh báo.
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
