import React, { useState, useEffect, useCallback } from 'react';
import { View, Text, StyleSheet, ActivityIndicator, RefreshControl, ScrollView } from 'react-native';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES } from '../../constants/theme';

/**
 * ==============================================================================
 * MÀN HÌNH: TỔNG QUAN (HOME SCREEN / DASHBOARD)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Đã tích hợp sẵn 100% bên dưới
 * ==============================================================================
 * 
 * 📌 DỮ LIỆU ĐÃ TỰ ĐỘNG TẢI TỪ BACKEND CÓ SẴN TRONG STATE:
 * 1. `walletsData`:
 *    - `walletsData.total_balance`: Tổng tài sản khả dụng (float)
 *    - `walletsData.wallets`: Mảng danh sách ví [{ id, name, balance, wallet_type }, ...]
 * 
 * 2. `budgetData`:
 *    - `budgetData.overall_budget.limit_amount`: Hạn mức tháng
 *    - `budgetData.overall_budget.spent_amount`: Số tiền đã chi
 *    - `budgetData.overall_budget.remaining_amount`: Tiền còn lại
 *    - `budgetData.overall_budget.percentage`: % đã tiêu
 *    - `budgetData.overall_budget.status_level`: 'NORMAL' | 'WARNING' | 'DANGER'
 *    - `budgetData.overall_budget.daily_safe_spending`: Mức chi an toàn mỗi ngày
 * 
 * 3. `recentTransactions`:
 *    - Mảng 5 giao dịch mới nhất [{ id, amount, transaction_type, category_name, note, transaction_date }, ...]
 * 
 * 4. Hàm tiện ích:
 *    - `onRefresh()`: Kéo xuống để nạp lại dữ liệu mới nhất
 *    - `formatCurrency(number)`: Định dạng tiền tệ VND (ví dụ: 15.000.000 ₫)
 * ==============================================================================
 */

export default function HomeScreen({ navigation }) {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [walletsData, setWalletsData] = useState({ total_balance: 0, wallets: [] });
  const [budgetData, setBudgetData] = useState(null);
  const [recentTransactions, setRecentTransactions] = useState([]);

  // Hàm tải dữ liệu tổng hợp cho trang chủ
  const fetchDashboardData = async () => {
    try {
      const [walletsRes, budgetRes, transRes] = await Promise.all([
        api.getWallets(),
        api.getBudgetStatus(),
        api.getTransactions({ limit: 5 })
      ]);

      if (walletsRes.data) setWalletsData(walletsRes.data);
      if (budgetRes.data) setBudgetData(budgetRes.data);
      if (transRes.data) setRecentTransactions(transRes.data.transactions || []);
    } catch (error) {
      console.warn('[HomeScreen] Lỗi tải dữ liệu dashboard:', error.message);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    fetchDashboardData();
  }, []);

  // Định dạng tiền tệ VND
  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount || 0);
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color={COLORS.primary} />
        <Text style={styles.loadingText}>Đang nạp dữ liệu tài chính...</Text>
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
        <Text style={styles.title}>Màn hình Tổng Quan (Dashboard)</Text>
        <Text style={styles.subtitle}>[Khu vực Trần Trọng Đại tự do thiết kế giao diện UI]</Text>
        
        <View style={styles.dataPreviewBox}>
          <Text style={styles.dataPreviewTitle}>Dữ liệu thực tế vừa nạp từ CSDL WAMP:</Text>
          <Text style={styles.dataPreviewText}>• Người dùng: {user?.full_name}</Text>
          <Text style={styles.dataPreviewText}>• Tổng tài sản: {formatCurrency(walletsData.total_balance)}</Text>
          <Text style={styles.dataPreviewText}>• Số lượng ví: {walletsData.wallets?.length || 0} ví</Text>
          <Text style={styles.dataPreviewText}>
            • Tiến độ ngân sách: {budgetData?.overall_budget?.percentage || 0}% ({budgetData?.overall_budget?.status_level || 'Chưa đặt'})
          </Text>
          <Text style={styles.dataPreviewText}>• Giao dịch gần đây: {recentTransactions.length} bản ghi</Text>
        </View>

        <Text style={styles.hint}>
          Đại xem hướng dẫn chi tiết các state và formatCurrency() trong file code này để vẽ giao diện.
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
  loadingText: {
    marginTop: SPACING.sm,
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
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
