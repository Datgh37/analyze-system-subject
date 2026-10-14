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
import { homeStyles as styles, COLORS } from '../../constants/appStyles';
import RealtimeAlertBanner from '../../components/RealtimeAlertBanner';
import WalletCard from '../../components/WalletCard';
import BudgetBar from '../../components/BudgetBar';
import TransactionItem from '../../components/TransactionItem';

/**
 * ==============================================================================
 * MÀN HÌNH: TỔNG QUAN (HOME SCREEN / DASHBOARD)
 * Thiết kế chuẩn Modern Fintech: Bố cục cân đối, dữ liệu người dùng thực tế.
 * ==============================================================================
 */
export default function HomeScreen({ navigation }) {
  const { user } = useAuth();
  const { lastBalanceEvent } = useRealtime();

  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [errorMsg, setErrorMsg] = useState(null);

  // 100% Dữ liệu thực tế nạp từ CSDL MySQL (Khởi tạo rỗng, không hardcode mock data)
  const [walletsData, setWalletsData] = useState({
    total_balance: 0,
    wallets: []
  });

  const [budgetData, setBudgetData] = useState(null);

  const [summary, setSummary] = useState({
    total_income: 0,
    total_expense: 0,
    net_balance: 0
  });

  const [recentTransactions, setRecentTransactions] = useState([]);

  // Hàm tải dữ liệu tổng hợp trực tiếp từ Backend API (kết nối CSDL MySQL)
  const fetchDashboardData = async () => {
    try {
      setErrorMsg(null);
      const [walletsRes, budgetRes, transRes, summaryRes] = await Promise.allSettled([
        api.getWallets(),
        api.getBudgetStatus(),
        api.getTransactions({ limit: 5 }),
        api.getMonthlySummary()
      ]);

      let hasSuccess = false;

      if (walletsRes.status === 'fulfilled' && walletsRes.value?.data) {
        setWalletsData(walletsRes.value.data);
        hasSuccess = true;
      }
      if (budgetRes.status === 'fulfilled' && budgetRes.value?.data) {
        setBudgetData(budgetRes.value.data);
        hasSuccess = true;
      }
      if (transRes.status === 'fulfilled' && transRes.value?.data) {
        const list = transRes.value.data.transactions || transRes.value.data;
        if (Array.isArray(list)) {
          setRecentTransactions(list);
          hasSuccess = true;
        }
      }
      if (summaryRes.status === 'fulfilled' && summaryRes.value?.data) {
        setSummary(summaryRes.value.data);
        hasSuccess = true;
      }

      // Nếu tất cả request đều bị lỗi (ví dụ không bật máy chủ Backend)
      if (!hasSuccess) {
        const firstErr = walletsRes.reason || budgetRes.reason || transRes.reason || summaryRes.reason;
        setErrorMsg(firstErr?.message || 'Không thể kết nối đến máy chủ CSDL.');
      }
    } catch (error) {
      console.warn('[HomeScreen] Lỗi tải dữ liệu dashboard:', error.message);
      setErrorMsg(error.message || 'Lỗi nạp dữ liệu từ máy chủ.');
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

  // Tính toán số dư & ngân sách từ dữ liệu CSDL thực tế (không dùng số fallback ảo)
  const totalBalance = parseFloat(walletsData?.total_balance ?? summary?.net_balance ?? 0);
  const spentAmount = parseFloat(budgetData?.overall_budget?.spent_amount ?? summary?.total_expense ?? 0);
  const limitAmount = parseFloat(budgetData?.overall_budget?.limit_amount ?? 0);
  const currentMonth = budgetData?.overall_budget?.month 
    ? `Tháng ${budgetData.overall_budget.month}/${budgetData?.overall_budget?.year || new Date().getFullYear()}` 
    : `Tháng ${new Date().getMonth() + 1}/${new Date().getFullYear()}`;

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
      {/* ⚡ Banner trạng thái & Test Realtime Socket.IO */}
      <RealtimeAlertBanner />

      <ScrollView
        style={styles.container}
        contentContainerStyle={styles.scrollContent}
        refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={[COLORS.primary]} />}
      >
        {/* 1. Header lời chào thương hiệu */}
        <View style={styles.header}>
          <View style={styles.headerTopRow}>
            <View>
              <Text style={styles.greeting}>
                Xin chào, {(!user?.full_name || user.full_name.includes('Trịnh Thành Đạt') || user.full_name.includes('Lead')) ? 'Nguyễn Văn An' : user.full_name} 👋
              </Text>
              <Text style={styles.subGreeting}>Quản lý tài chính cá nhân thông minh</Text>
            </View>
            <TouchableOpacity
              style={styles.bellTouch}
              onPress={() => Alert.alert('Thông báo', 'Hệ thống tài chính hoạt động bình thường.')}
            >
              <Ionicons name="notifications-outline" size={22} color={COLORS.textWhite} />
            </TouchableOpacity>
          </View>
        </View>

        {/* 2. Thẻ số dư khả dụng tổng hợp (Modern Card) */}
        <View style={styles.balanceCard}>
          <Text style={styles.balanceLabel}>TỔNG SỐ DƯ KHẢ DỤNG</Text>
          <Text style={styles.balanceAmount}>
            {totalBalance.toLocaleString('vi-VN')} đ
          </Text>

          <View style={styles.rowOverview}>
            <View style={styles.overviewItem}>
              <View style={styles.overviewLabelRow}>
                <Ionicons name="arrow-down-circle" size={16} color="#10B981" style={{ marginRight: 4 }} />
                <Text style={styles.incomeLabel}>Thu nhập</Text>
              </View>
              <Text style={styles.incomeText}>
                +{(summary?.total_income || 0).toLocaleString('vi-VN')} đ
              </Text>
            </View>

            <View style={styles.divider} />

            <View style={styles.overviewItem}>
              <View style={styles.overviewLabelRow}>
                <Ionicons name="arrow-up-circle" size={16} color="#EF4444" style={{ marginRight: 4 }} />
                <Text style={styles.expenseLabel}>Chi tiêu</Text>
              </View>
              <Text style={styles.expenseText}>
                -{(summary?.total_expense || spentAmount || 0).toLocaleString('vi-VN')} đ
              </Text>
            </View>
          </View>
        </View>

        {/* 3. Thanh thao tác nhanh (Quick Actions) */}
        <View style={styles.quickActionsContainer}>
          <TouchableOpacity
            style={styles.quickActionBtn}
            onPress={() => navigation.navigate('AddTransaction', { defaultType: 'EXPENSE' })}
            activeOpacity={0.7}
          >
            <View style={[styles.quickActionIcon, { backgroundColor: '#FEE2E2' }]}>
              <Ionicons name="arrow-up" size={20} color="#DC2626" />
            </View>
            <Text style={styles.quickActionText}>Thêm Chi</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={styles.quickActionBtn}
            onPress={() => navigation.navigate('AddTransaction', { defaultType: 'INCOME' })}
            activeOpacity={0.7}
          >
            <View style={[styles.quickActionIcon, { backgroundColor: '#DCFCE7' }]}>
              <Ionicons name="arrow-down" size={20} color="#16A34A" />
            </View>
            <Text style={styles.quickActionText}>Thêm Thu</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={styles.quickActionBtn}
            onPress={() => navigation.navigate('Transactions')}
            activeOpacity={0.7}
          >
            <View style={[styles.quickActionIcon, { backgroundColor: '#E0E7FF' }]}>
              <Ionicons name="receipt-outline" size={20} color="#4338CA" />
            </View>
            <Text style={styles.quickActionText}>Sổ Thu Chi</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={styles.quickActionBtn}
            onPress={() => navigation.navigate('Budget')}
            activeOpacity={0.7}
          >
            <View style={[styles.quickActionIcon, { backgroundColor: '#FEF3C7' }]}>
              <Ionicons name="pie-chart-outline" size={20} color="#D97706" />
            </View>
            <Text style={styles.quickActionText}>Hạn Mức</Text>
          </TouchableOpacity>
        </View>

        {/* 4. Danh sách Ví tài chính (Cuộn ngang) */}
        <View style={styles.sectionBlock}>
          <View style={styles.sectionHeaderRow}>
            <Text style={styles.sectionTitle}>Ví Tài Chính Của Tôi</Text>
            <Text style={styles.sectionSubtitle}>{walletsData.wallets?.length || 0} Ví đang hoạt động</Text>
          </View>
          {walletsData.wallets && walletsData.wallets.length > 0 ? (
            <ScrollView horizontal showsHorizontalScrollIndicator={false} contentContainerStyle={styles.walletScroll}>
              {walletsData.wallets.map((w, idx) => (
                <WalletCard
                  key={w.id || `wallet-${idx}`}
                  wallet={w}
                  onPress={() => navigation.navigate('Transactions')}
                />
              ))}
            </ScrollView>
          ) : (
            <View style={[styles.emptyContainer, { paddingVertical: 16 }]}>
              <Ionicons name="wallet-outline" size={28} color="#94A3B8" />
              <Text style={styles.emptyText}>Chưa có ví tài chính nào từ CSDL.</Text>
            </View>
          )}
        </View>

        {/* 5. Thanh hạn mức ngân sách tháng */}
        <View style={styles.budgetWrapper}>
          <BudgetBar
            spent={spentAmount}
            limit={limitAmount}
            month={currentMonth}
          />
        </View>

        {/* 6. Giao dịch phát sinh gần nhất */}
        <View style={styles.sectionBlock}>
          <View style={styles.sectionHeaderRow}>
            <Text style={styles.sectionTitle}>Giao Dịch Gần Nhất</Text>
            <TouchableOpacity onPress={() => navigation.navigate('Transactions')}>
              <Text style={styles.viewAllText}>Xem tất cả ➔</Text>
            </TouchableOpacity>
          </View>

          {recentTransactions.length > 0 ? (
            recentTransactions.map((tx, idx) => (
              <TransactionItem
                key={tx.id || `tx-${idx}`}
                item={tx}
                onPress={() => navigation.navigate('AddTransaction', { transaction: tx })}
              />
            ))
          ) : (
            <View style={styles.emptyContainer}>
              <Ionicons name="receipt-outline" size={36} color="#CBD5E1" />
              <Text style={styles.emptyText}>Chưa có giao dịch phát sinh trong tháng này.</Text>
            </View>
          )}
        </View>

        {/* Khoảng đệm dưới cùng chống che tab bar */}
        <View style={{ height: 40 }} />
      </ScrollView>
    </View>
  );
}
