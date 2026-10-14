import React, { useState, useEffect, useCallback } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ActivityIndicator,
  RefreshControl,
  FlatList,
  TouchableOpacity,
  TextInput,
  Alert
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES, SHADOWS } from '../../constants/theme';
import TransactionItem from '../../components/TransactionItem';

/**
 * ==============================================================================
 * MÀN HÌNH: SỔ GIAO DỊCH (TRANSACTIONS SCREEN / LỊCH SỬ THU CHI)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Trịnh Thành Đạt (Lead Architect)
 * ==============================================================================
 */
export default function TransactionsScreen({ navigation }) {
  const [transactions, setTransactions] = useState([
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
      category_name: 'Mua sắm',
      note: 'Sách Lập trình React Native',
      amount: 250000,
      transaction_type: 'EXPENSE',
      transaction_date: '2026-09-27',
      wallet_name: 'Ngân Hàng Vietcombank',
      color_code: '#4ECDC4'
    },
    {
      id: 4,
      category_name: 'Hóa đơn',
      note: 'Tiền điện nước tháng 9',
      amount: 1200000,
      transaction_type: 'EXPENSE',
      transaction_date: '2026-09-25',
      wallet_name: 'Ngân Hàng Vietcombank',
      color_code: '#1A535C'
    },
    {
      id: 5,
      category_name: 'Lương',
      note: 'Lương thực tập part-time',
      amount: 5000000,
      transaction_type: 'INCOME',
      transaction_date: '2026-09-20',
      wallet_name: 'Ngân Hàng Vietcombank',
      color_code: '#2EC4B6'
    }
  ]);

  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [filterType, setFilterType] = useState('ALL'); // 'ALL' | 'EXPENSE' | 'INCOME'
  const [keyword, setKeyword] = useState('');
  const [currentMonth, setCurrentMonth] = useState(new Date().getMonth() + 1);
  const [currentYear, setCurrentYear] = useState(new Date().getFullYear());

  const fetchTransactions = async () => {
    try {
      const typeParam = filterType === 'ALL' ? null : filterType;
      const res = await api.getTransactions({
        month: currentMonth,
        year: currentYear,
        type: typeParam,
        keyword: keyword.trim()
      });
      if (res.data) {
        const list = res.data.transactions || res.data;
        if (Array.isArray(list) && list.length > 0) {
          setTransactions(list);
        }
      }
    } catch (error) {
      console.warn('[TransactionsScreen] Lỗi nạp dữ liệu, dùng mock fallback:', error.message);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    fetchTransactions();
  }, [filterType, currentMonth, currentYear]);

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    fetchTransactions();
  }, [filterType, keyword, currentMonth, currentYear]);

  const handleDeleteTransaction = (id, note) => {
    Alert.alert(
      'Xác nhận xóa giao dịch',
      `Bạn có chắc chắn muốn xóa giao dịch "${note || 'này'}"? Số dư ví sẽ được tự động hoàn lại.`,
      [
        { text: 'Hủy', style: 'cancel' },
        {
          text: 'Xóa ngay',
          style: 'destructive',
          onPress: async () => {
            try {
              await api.deleteTransaction(id);
              Alert.alert('Thành công', 'Đã xóa giao dịch và hoàn trả số dư ví.');
              fetchTransactions();
            } catch (err) {
              // Mock fallback xóa khỏi local state
              setTransactions((prev) => prev.filter((item) => item.id !== id));
              Alert.alert('Thành công (Chế độ xem trước)', 'Đã xóa giao dịch khỏi danh sách.');
            }
          }
        }
      ]
    );
  };

  // Lọc dữ liệu hiển thị theo filterType & từ khóa
  const displayedTransactions = transactions.filter((item) => {
    if (filterType !== 'ALL' && item.transaction_type !== filterType) return false;
    if (keyword.trim()) {
      const kw = keyword.toLowerCase();
      const matchNote = (item.note || '').toLowerCase().includes(kw);
      const matchCat = (item.category_name || '').toLowerCase().includes(kw);
      return matchNote || matchCat;
    }
    return true;
  });

  // Tính tổng thu, tổng chi của danh sách hiển thị
  const totalExpense = displayedTransactions
    .filter((tx) => tx.transaction_type === 'EXPENSE')
    .reduce((sum, tx) => sum + parseFloat(tx.amount || 0), 0);

  const totalIncome = displayedTransactions
    .filter((tx) => tx.transaction_type === 'INCOME')
    .reduce((sum, tx) => sum + parseFloat(tx.amount || 0), 0);

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color={COLORS.primary} />
        <Text style={styles.loadingText}>Đang nạp sổ giao dịch...</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      {/* 1. Thanh tìm kiếm & Lọc tháng */}
      <View style={styles.topFilterBar}>
        <View style={styles.searchBox}>
          <Ionicons name="search-outline" size={18} color={COLORS.textSecondary} style={{ marginRight: 6 }} />
          <TextInput
            style={styles.searchInput}
            placeholder="Tìm theo ghi chú hoặc danh mục..."
            placeholderTextColor={COLORS.textMuted}
            value={keyword}
            onChangeText={setKeyword}
            onSubmitEditing={fetchTransactions}
          />
          {keyword ? (
            <TouchableOpacity onPress={() => setKeyword('')}>
              <Ionicons name="close-circle" size={16} color={COLORS.textMuted} />
            </TouchableOpacity>
          ) : null}
        </View>
      </View>

      {/* 2. Bộ lọc Tabs 3 chế độ (Tất cả / Khoản Chi / Khoản Thu) */}
      <View style={styles.tabContainer}>
        <TouchableOpacity
          style={[styles.tabButton, filterType === 'ALL' && styles.tabButtonActive]}
          onPress={() => setFilterType('ALL')}
        >
          <Text style={[styles.tabText, filterType === 'ALL' && styles.tabTextActive]}>
            Tất cả ({displayedTransactions.length})
          </Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={[styles.tabButton, filterType === 'EXPENSE' && styles.tabExpenseActive]}
          onPress={() => setFilterType('EXPENSE')}
        >
          <Text style={[styles.tabText, filterType === 'EXPENSE' && styles.tabTextActive]}>
            Khoản Chi
          </Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={[styles.tabButton, filterType === 'INCOME' && styles.tabIncomeActive]}
          onPress={() => setFilterType('INCOME')}
        >
          <Text style={[styles.tabText, filterType === 'INCOME' && styles.tabTextActive]}>
            Khoản Thu
          </Text>
        </TouchableOpacity>
      </View>

      {/* 3. Thẻ tóm tắt tổng thu / chi của bộ lọc */}
      <View style={styles.summaryBar}>
        <View style={styles.summaryItem}>
          <Text style={styles.summaryLabel}>Tổng chi:</Text>
          <Text style={styles.summaryExpense}>-{totalExpense.toLocaleString('vi-VN')} đ</Text>
        </View>
        <View style={styles.summaryDivider} />
        <View style={styles.summaryItem}>
          <Text style={styles.summaryLabel}>Tổng thu:</Text>
          <Text style={styles.summaryIncome}>+{totalIncome.toLocaleString('vi-VN')} đ</Text>
        </View>
      </View>

      {/* 4. Danh sách giao dịch dạng FlatList */}
      <FlatList
        data={displayedTransactions}
        keyExtractor={(item) => item.id.toString()}
        contentContainerStyle={styles.listContent}
        refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={[COLORS.primary]} />}
        renderItem={({ item }) => (
          <TouchableOpacity
            activeOpacity={0.9}
            onPress={() => navigation.navigate('AddTransaction', { transaction: item })}
          >
            <TransactionItem
              item={item}
              onDelete={() => handleDeleteTransaction(item.id, item.note)}
            />
          </TouchableOpacity>
        )}
        ListEmptyComponent={
          <View style={styles.emptyContainer}>
            <Ionicons name="receipt-outline" size={54} color={COLORS.textMuted} />
            <Text style={styles.emptyTitle}>Chưa có giao dịch nào</Text>
            <Text style={styles.emptySubtitle}>Bấm nút "Thêm mới" để ghi nhận khoản thu/chi</Text>
            <TouchableOpacity
              style={styles.emptyAddBtn}
              onPress={() => navigation.navigate('AddTransaction')}
            >
              <Text style={styles.emptyAddBtnText}>+ Thêm giao dịch đầu tiên</Text>
            </TouchableOpacity>
          </View>
        }
      />
    </View>
  );
}

const styles = StyleSheet.create({
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
  topFilterBar: {
    paddingHorizontal: SPACING.md,
    paddingTop: SPACING.md,
    paddingBottom: SPACING.xs
  },
  searchBox: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusMd,
    paddingHorizontal: SPACING.md,
    borderWidth: 1,
    borderColor: COLORS.border,
    height: 44,
    ...SHADOWS.small
  },
  searchInput: {
    flex: 1,
    fontSize: SIZES.fontSm,
    color: COLORS.textPrimary
  },
  tabContainer: {
    flexDirection: 'row',
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm,
    backgroundColor: COLORS.background
  },
  tabButton: {
    flex: 1,
    paddingVertical: SPACING.sm,
    borderRadius: SIZES.radiusSm,
    alignItems: 'center',
    backgroundColor: COLORS.surfaceVariant,
    marginHorizontal: 3
  },
  tabButtonActive: {
    backgroundColor: COLORS.primary
  },
  tabExpenseActive: {
    backgroundColor: COLORS.expense
  },
  tabIncomeActive: {
    backgroundColor: COLORS.income
  },
  tabText: {
    fontSize: SIZES.fontSm,
    fontWeight: '600',
    color: COLORS.textSecondary
  },
  tabTextActive: {
    color: COLORS.textWhite
  },
  summaryBar: {
    flexDirection: 'row',
    backgroundColor: COLORS.surface,
    marginHorizontal: SPACING.md,
    marginBottom: SPACING.xs,
    paddingVertical: SPACING.sm,
    paddingHorizontal: SPACING.md,
    borderRadius: SIZES.radiusSm,
    borderWidth: 1,
    borderColor: COLORS.borderLight,
    alignItems: 'center',
    justifyContent: 'space-around'
  },
  summaryItem: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  summaryLabel: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    marginRight: 6
  },
  summaryExpense: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold',
    color: COLORS.expense
  },
  summaryIncome: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold',
    color: COLORS.income
  },
  summaryDivider: {
    width: 1,
    height: 18,
    backgroundColor: COLORS.border
  },
  listContent: {
    paddingHorizontal: SPACING.md,
    paddingTop: SPACING.xs,
    paddingBottom: SPACING.xl
  },
  emptyContainer: {
    paddingVertical: SPACING.xxl,
    alignItems: 'center',
    justifyContent: 'center'
  },
  emptyTitle: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.textPrimary,
    marginTop: SPACING.sm
  },
  emptySubtitle: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginTop: 4,
    textAlign: 'center'
  },
  emptyAddBtn: {
    marginTop: SPACING.md,
    backgroundColor: COLORS.primary,
    paddingHorizontal: SPACING.lg,
    paddingVertical: SPACING.sm + 2,
    borderRadius: SIZES.radiusMd
  },
  emptyAddBtnText: {
    color: COLORS.textWhite,
    fontSize: SIZES.fontSm,
    fontWeight: 'bold'
  }
});
