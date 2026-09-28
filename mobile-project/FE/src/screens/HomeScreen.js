import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ScrollView, TouchableOpacity, ActivityIndicator } from 'react-native';
import { COLORS, SIZES } from '../constants/theme';
import apiClient from '../api/client';

/**
 * MÀN HÌNH TRANG CHỦ (HOME DASHBOARD)
 * Kiến trúc & Kết nối API: Trịnh Thành Đạt (Lead)
 * Phụ trách hoàn thiện UI & Tách Components: Trần Trọng Đại (UI Specialist)
 * 
 * 👉 Nhiệm vụ Đại add nốt:
 * - Tách thẻ ví thành linh kiện tái sử dụng: src/components/WalletCard.js
 * - Tách thanh ngân sách thành: src/components/BudgetBar.js
 */
export default function HomeScreen({ navigation }) {
  const [loading, setLoading] = useState(true);
  const [summary, setSummary] = useState({
    total_income: 0,
    total_expense: 0,
    net_balance: 0,
    budget_limit: 0,
    spent_percentage: 0
  });

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      const res = await apiClient.get('/reports/monthly_summary.php?user_id=1&month=2026-09');
      if (res && res.data) {
        setSummary(res.data);
      }
    } catch (error) {
      console.log('Lỗi tải dữ liệu dashboard:', error.message);
    } finally {
      setLoading(false);
    }
  };

  const getBudgetColor = (percent) => {
    if (percent < 70) return COLORS.income;
    if (percent <= 90) return COLORS.warning;
    return COLORS.danger;
  };

  return (
    <ScrollView style={styles.container}>
      {/* 1. Header lời chào */}
      <View style={styles.header}>
        <Text style={styles.greeting}>Xin chào, Trịnh Thành Đạt 👋</Text>
        <Text style={styles.subGreeting}>Quản lý chi tiêu thông minh tháng 09/2026</Text>
      </View>

      {/* 2. Thẻ hiển thị số dư ròng */}
      <View style={styles.balanceCard}>
        <Text style={styles.balanceLabel}>Số Dư Khả Dụng Hiện Tại</Text>
        <Text style={styles.balanceAmount}>
          {summary.net_balance.toLocaleString('vi-VN')} đ
        </Text>
        <View style={styles.rowOverview}>
          <View style={styles.overviewItem}>
            <Text style={styles.incomeLabel}>↓ Thu nhập</Text>
            <Text style={styles.incomeText}>+{summary.total_income.toLocaleString('vi-VN')} đ</Text>
          </View>
          <View style={styles.divider} />
          <View style={styles.overviewItem}>
            <Text style={styles.expenseLabel}>↑ Chi tiêu</Text>
            <Text style={styles.expenseText}>-{summary.total_expense.toLocaleString('vi-VN')} đ</Text>
          </View>
        </View>
      </View>

      {/* 3. Khung ngân sách tháng */}
      <View style={styles.card}>
        <View style={styles.cardHeader}>
          <Text style={styles.cardTitle}>Hạn Mức Ngân Sách Tháng</Text>
          <Text style={styles.percentText}>{summary.spent_percentage}%</Text>
        </View>
        <View style={styles.progressBarBackground}>
          <View
            style={[
              styles.progressBarFill,
              {
                width: `${Math.min(summary.spent_percentage, 100)}%`,
                backgroundColor: getBudgetColor(summary.spent_percentage)
              }
            ]}
          />
        </View>
        <Text style={styles.budgetHelper}>
          Đã chi: {summary.total_expense.toLocaleString('vi-VN')} đ / {summary.budget_limit.toLocaleString('vi-VN')} đ
        </Text>
      </View>

      {/* 4. Nút thêm nhanh */}
      <View style={styles.actionRow}>
        <TouchableOpacity
          style={styles.actionButton}
          onPress={() => navigation.navigate('Thêm Mới')}
        >
          <Text style={styles.actionButtonText}>+ Thêm Khoản Thu / Chi</Text>
        </TouchableOpacity>
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background
  },
  header: {
    backgroundColor: COLORS.primary,
    paddingHorizontal: SIZES.md,
    paddingTop: SIZES.lg,
    paddingBottom: SIZES.xl,
    borderBottomLeftRadius: SIZES.radiusLg,
    borderBottomRightRadius: SIZES.radiusLg
  },
  greeting: {
    fontSize: SIZES.fontXl,
    fontWeight: 'bold',
    color: COLORS.white
  },
  subGreeting: {
    fontSize: SIZES.fontMd,
    color: COLORS.primaryLight,
    marginTop: SIZES.xs
  },
  balanceCard: {
    backgroundColor: COLORS.card,
    marginHorizontal: SIZES.md,
    marginTop: -SIZES.md,
    borderRadius: SIZES.radiusMd,
    padding: SIZES.md,
    elevation: 3
  },
  balanceLabel: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  balanceAmount: {
    fontSize: SIZES.fontTitle,
    fontWeight: 'bold',
    color: COLORS.text,
    marginVertical: SIZES.xs
  },
  rowOverview: {
    flexDirection: 'row',
    marginTop: SIZES.md,
    paddingTop: SIZES.md,
    borderTopWidth: 1,
    borderTopColor: COLORS.border
  },
  overviewItem: {
    flex: 1
  },
  divider: {
    width: 1,
    backgroundColor: COLORS.border,
    marginHorizontal: SIZES.sm
  },
  incomeLabel: {
    fontSize: SIZES.fontSm,
    color: COLORS.income
  },
  incomeText: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.income,
    marginTop: 2
  },
  expenseLabel: {
    fontSize: SIZES.fontSm,
    color: COLORS.expense
  },
  expenseText: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.expense,
    marginTop: 2
  },
  card: {
    backgroundColor: COLORS.card,
    marginHorizontal: SIZES.md,
    marginTop: SIZES.md,
    borderRadius: SIZES.radiusMd,
    padding: SIZES.md,
    elevation: 2
  },
  cardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: SIZES.sm
  },
  cardTitle: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.text
  },
  percentText: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.textSecondary
  },
  progressBarBackground: {
    height: 10,
    backgroundColor: COLORS.border,
    borderRadius: SIZES.radiusFull,
    overflow: 'hidden'
  },
  progressBarFill: {
    height: '100%',
    borderRadius: SIZES.radiusFull
  },
  budgetHelper: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginTop: SIZES.sm
  },
  actionRow: {
    padding: SIZES.md,
    marginTop: SIZES.sm,
    marginBottom: SIZES.xl
  },
  actionButton: {
    backgroundColor: COLORS.primary,
    paddingVertical: SIZES.md,
    borderRadius: SIZES.radiusMd,
    alignItems: 'center'
  },
  actionButtonText: {
    color: COLORS.white,
    fontSize: SIZES.fontLg,
    fontWeight: 'bold'
  }
});
