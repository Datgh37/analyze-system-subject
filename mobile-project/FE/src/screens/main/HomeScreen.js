import React, { useState, useEffect, useCallback } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ActivityIndicator,
  RefreshControl,
  ScrollView,
  TouchableOpacity,
  Alert
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useAuth } from '../../context/AuthContext';
import { useRealtime } from '../../context/RealtimeContext';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES, SHADOWS } from '../../constants/theme';
import RealtimeAlertBanner from '../../components/RealtimeAlertBanner';
import WalletCard from '../../components/WalletCard';
import BudgetBar from '../../components/BudgetBar';
import TransactionItem from '../../components/TransactionItem';

/**
 * ==============================================================================
 * MÀN HÌNH: TỔNG QUAN (HOME SCREEN / DASHBOARD)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Tích hợp Kiến trúc & Realtime: Trịnh Thành Đạt (Lead Architect)
 * ==============================================================================
 */
export default function HomeScreen({ navigation }) {
  const { user } = useAuth();
  const { lastBalanceEvent } = useRealtime();

  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  // Dữ liệu ví & tổng tài sản
  const [walletsData, setWalletsData] = useState({
    total_balance: 18700000,
    wallets: [
      { id: 1, name: 'Ví Tiền Mặt', balance: 3500000, wallet_type: 'CASH' },
      { id: 2, name: 'Ngân Hàng Vietcombank', balance: 15200000, wallet_type: 'BANK' }
    ]
  });

  // Dữ liệu ngân sách tháng
  const [budgetData, setBudgetData] = useState({
    overall_budget: {
      limit_amount: 12000000,
      spent_amount: 8500000,
      remaining_amount: 3500000,
      percentage: 71,
      status_level: 'WARNING',
      month: '09/2026'
    }
  });

  // Dữ liệu tổng hợp thu chi tháng
  const [summary, setSummary] = useState({
    total_income: 15000000,
    total_expense: 8500000,
    net_balance: 18700000
  });

  // Dữ liệu 5 giao dịch gần nhất
  const [recentTransactions, setRecentTransactions] = useState([
    {
      id: 1,
      category_name: 'Ăn uống',
      note: 'Cơm trưa văn phòng',
      amount: 45000,
      transaction_type: 'EXPENSE',
      transaction_date: '2026-09-29',
      wallet_name: 'Ví Tiền Mặt',
      color_code: '#FF6B6B'
    },
    {
      id: 2,
      category_name: 'Đi lại',
      note: 'Đổ xăng xe máy',
      amount: 80000,
      transaction_type: 'EXPENSE',
      transaction_date: '2026-09-28',
      wallet_name: 'Ví Tiền Mặt',
      color_code: '#FFE66D'
    },
    {
      id: 3,
      category_name: 'Lương',
      note: 'Lương thực tập tháng 09',
      amount: 7000000,
      transaction_type: 'INCOME',
      transaction_date: '2026-09-25',
      wallet_name: 'Ngân Hàng Vietcombank',
      color_code: '#2EC4B6'
    }
  ]);

  // Hàm tải dữ liệu tổng hợp cho trang chủ
  const fetchDashboardData = async () => {
    try {
      const [walletsRes, budgetRes, transRes, summaryRes] = await Promise.allSettled([
        api.getWallets(),
        api.getBudgetStatus(),
        api.getTransactions({ limit: 5 }),
        api.getMonthlySummary()
      ]);

      if (walletsRes.status === 'fulfilled' && walletsRes.value?.data) {
        setWalletsData(walletsRes.value.data);
      }
      if (budgetRes.status === 'fulfilled' && budgetRes.value?.data) {
        setBudgetData(budgetRes.value.data);
      }
      if (transRes.status === 'fulfilled' && transRes.value?.data) {
        const list = transRes.value.data.transactions || transRes.value.data;
        if (Array.isArray(list) && list.length > 0) {
          setRecentTransactions(list);
        }
      }
      if (summaryRes.status === 'fulfilled' && summaryRes.value?.data) {
        setSummary(summaryRes.value.data);
      }
    } catch (error) {
      console.warn('[HomeScreen] Lỗi tải dữ liệu dashboard, sử dụng mock fallback:', error.message);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    fetchDashboardData();
  }, []);

  // ⚡ Tự động nạp lại Dashboard khi nhận sự kiện biến động số dư từ Socket.IO (0ms sync)
  useEffect(() => {
    if (lastBalanceEvent) {
      console.log('⚡ [HomeScreen] Tự động cập nhật Dashboard theo Socket.IO:', lastBalanceEvent);
      fetchDashboardData();
    }
  }, [lastBalanceEvent]);

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    fetchDashboardData();
  }, []);

  const totalBalance = walletsData.total_balance ?? summary.net_balance ?? 0;
  const spentAmount = budgetData?.overall_budget?.spent_amount ?? summary.total_expense ?? 0;
  const limitAmount = budgetData?.overall_budget?.limit_amount ?? 12000000;
  const currentMonth = budgetData?.overall_budget?.month || 'Tháng 10/2026';

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color={COLORS.primary} />
        <Text style={styles.loadingText}>Đang nạp dữ liệu tài chính...</Text>
      </View>
    );
  }

  return (
    <View style={styles.screenWrapper}>
      {/* ⚡ Banner cảnh báo & trạng thái thời gian thực Socket.IO */}
      <RealtimeAlertBanner />

      <ScrollView
        style={styles.container}
        refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={[COLORS.primary]} />}
      >
        {/* 1. Header lời chào & Thông báo */}
        <View style={styles.header}>
          <View style={styles.headerTopRow}>
            <View>
              <Text style={styles.greeting}>
                Xin chào, {user?.full_name || 'Trần Trọng Đại'} 👋
              </Text>
              <Text style={styles.subGreeting}>Quản lý tài chính cá nhân thông minh</Text>
            </View>
            <TouchableOpacity
              style={styles.bellTouch}
              onPress={() => Alert.alert('Thông báo', 'Hệ thống hoạt động bình thường.')}
            >
              <Ionicons name="notifications-outline" size={22} color={COLORS.textWhite} />
            </TouchableOpacity>
          </View>
        </View>

        {/* 2. Thẻ số dư khả dụng tổng hợp */}
        <View style={styles.balanceCard}>
          <Text style={styles.balanceLabel}>TỔNG SỐ DƯ KHẢ DỤNG</Text>
          <Text style={styles.balanceAmount}>
            {totalBalance.toLocaleString('vi-VN')} đ
          </Text>

          <View style={styles.rowOverview}>
            <View style={styles.overviewItem}>
              <View style={styles.overviewLabelRow}>
                <Ionicons name="arrow-down-circle" size={16} color={COLORS.income} style={{ marginRight: 4 }} />
                <Text style={styles.incomeLabel}>Thu nhập</Text>
              </View>
              <Text style={styles.incomeText}>
                +{(summary.total_income || 0).toLocaleString('vi-VN')} đ
              </Text>
            </View>

            <View style={styles.divider} />

            <View style={styles.overviewItem}>
              <View style={styles.overviewLabelRow}>
                <Ionicons name="arrow-up-circle" size={16} color={COLORS.expense} style={{ marginRight: 4 }} />
                <Text style={styles.expenseLabel}>Chi tiêu</Text>
              </View>
              <Text style={styles.expenseText}>
                -{(summary.total_expense || spentAmount).toLocaleString('vi-VN')} đ
              </Text>
            </View>
          </View>
        </View>

        {/* 3. Danh sách Ví tài chính (Cuộn ngang) */}
        <View style={styles.sectionBlock}>
          <View style={styles.sectionHeaderRow}>
            <Text style={styles.sectionTitle}>Ví Tài Chính Của Tôi</Text>
            <Text style={styles.sectionSubtitle}>{walletsData.wallets?.length || 0} Ví đang hoạt động</Text>
          </View>
          <ScrollView horizontal showsHorizontalScrollIndicator={false} contentContainerStyle={styles.walletScroll}>
            {(walletsData.wallets || []).map((w) => (
              <WalletCard
                key={w.id}
                wallet={w}
                onPress={() => navigation.navigate('Transactions')}
              />
            ))}
          </ScrollView>
        </View>

        {/* 4. Thanh hạn mức ngân sách tháng */}
        <View style={styles.budgetWrapper}>
          <BudgetBar
            spent={spentAmount}
            limit={limitAmount}
            month={currentMonth}
          />
        </View>

        {/* 5. Giao dịch phát sinh gần nhất */}
        <View style={styles.sectionBlock}>
          <View style={styles.sectionHeaderRow}>
            <Text style={styles.sectionTitle}>Giao Dịch Gần Nhất</Text>
            <TouchableOpacity onPress={() => navigation.navigate('Transactions')}>
              <Text style={styles.viewAllText}>Xem tất cả ➔</Text>
            </TouchableOpacity>
          </View>

          {recentTransactions.map((tx) => (
            <TransactionItem key={tx.id} item={tx} />
          ))}
        </View>

        {/* 6. Nút Thêm Giao Dịch Nhanh */}
        <View style={styles.actionRow}>
          <TouchableOpacity
            style={styles.actionButton}
            onPress={() => navigation.navigate('AddTransaction')}
          >
            <Ionicons name="add-circle" size={22} color={COLORS.textWhite} style={{ marginRight: 6 }} />
            <Text style={styles.actionButtonText}>Thêm Khoản Thu / Chi Mới</Text>
          </TouchableOpacity>
        </View>
      </ScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  screenWrapper: {
    flex: 1,
    backgroundColor: COLORS.background
  },
  container: {
    flex: 1,
    backgroundColor: COLORS.background
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
  header: {
    backgroundColor: COLORS.primary,
    paddingHorizontal: SPACING.md,
    paddingTop: SPACING.lg + 8,
    paddingBottom: SPACING.xl + 12,
    borderBottomLeftRadius: SIZES.radiusLg,
    borderBottomRightRadius: SIZES.radiusLg
  },
  headerTopRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center'
  },
  greeting: {
    fontSize: SIZES.fontXl,
    fontWeight: 'bold',
    color: COLORS.textWhite
  },
  subGreeting: {
    fontSize: SIZES.fontSm,
    color: COLORS.primaryLight,
    marginTop: 2
  },
  bellTouch: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: 'rgba(255,255,255,0.2)',
    justifyContent: 'center',
    alignItems: 'center'
  },
  balanceCard: {
    backgroundColor: COLORS.surface,
    marginHorizontal: SPACING.md,
    marginTop: -SPACING.xl,
    borderRadius: SIZES.radiusMd,
    padding: SPACING.md,
    borderWidth: 1,
    borderColor: COLORS.border,
    ...SHADOWS.card
  },
  balanceLabel: {
    fontSize: SIZES.fontXs,
    fontWeight: 'bold',
    color: COLORS.textSecondary,
    letterSpacing: 0.5
  },
  balanceAmount: {
    fontSize: SIZES.fontXxl,
    fontWeight: 'bold',
    color: COLORS.textPrimary,
    marginVertical: 4
  },
  rowOverview: {
    flexDirection: 'row',
    marginTop: SPACING.sm,
    paddingTop: SPACING.sm,
    borderTopWidth: 1,
    borderTopColor: COLORS.border
  },
  overviewItem: {
    flex: 1
  },
  overviewLabelRow: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  divider: {
    width: 1,
    backgroundColor: COLORS.border,
    marginHorizontal: SPACING.sm
  },
  incomeLabel: {
    fontSize: SIZES.fontSm - 1,
    color: COLORS.income,
    fontWeight: '600'
  },
  incomeText: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.income,
    marginTop: 2
  },
  expenseLabel: {
    fontSize: SIZES.fontSm - 1,
    color: COLORS.expense,
    fontWeight: '600'
  },
  expenseText: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.expense,
    marginTop: 2
  },
  sectionBlock: {
    marginTop: SPACING.lg,
    paddingHorizontal: SPACING.md
  },
  sectionHeaderRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: SPACING.sm
  },
  sectionTitle: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.textPrimary
  },
  sectionSubtitle: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary
  },
  viewAllText: {
    fontSize: SIZES.fontSm,
    color: COLORS.primary,
    fontWeight: 'bold'
  },
  walletScroll: {
    paddingVertical: 4
  },
  budgetWrapper: {
    marginTop: SPACING.md,
    paddingHorizontal: SPACING.md
  },
  actionRow: {
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.lg,
    marginBottom: SPACING.xl
  },
  actionButton: {
    backgroundColor: COLORS.primary,
    flexDirection: 'row',
    paddingVertical: SPACING.md - 2,
    borderRadius: SIZES.radiusMd,
    alignItems: 'center',
    justifyContent: 'center',
    ...SHADOWS.small
  },
  actionButtonText: {
    color: COLORS.textWhite,
    fontSize: SIZES.fontMd,
    fontWeight: 'bold'
  }
});
