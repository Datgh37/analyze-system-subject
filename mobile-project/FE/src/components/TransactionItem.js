import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { COLORS, SIZES } from '../constants/theme';

/**
 * LINH KIỆN DÒNG GIAO DỊCH THU / CHI (TRANSACTION ITEM)
 * Sinh viên thực hiện: Trần Trọng Đại (UI Specialist)
 * Phục vụ: Danh sách giao dịch ở HomeScreen và HistoryScreen
 */
export default function TransactionItem({ item, onDelete }) {
  const isExpense = item.transaction_type === 'EXPENSE';
  const amount = typeof item.amount === 'number' ? item.amount : parseFloat(item.amount || 0);

  // Ánh xạ icon danh mục
  const getCategoryIcon = (categoryName) => {
    const name = (categoryName || '').toLowerCase();
    if (name.includes('ăn') || name.includes('uống') || name.includes('food')) return 'restaurant-outline';
    if (name.includes('xe') || name.includes('xăng') || name.includes('đi lại')) return 'car-outline';
    if (name.includes('mua') || name.includes('sắm') || name.includes('shopping')) return 'cart-outline';
    if (name.includes('nhà') || name.includes('ở') || name.includes('tiền phòng')) return 'home-outline';
    if (name.includes('hóa đơn') || name.includes('điện') || name.includes('nước')) return 'receipt-outline';
    if (name.includes('lương') || name.includes('salary')) return 'cash-outline';
    if (name.includes('thưởng') || name.includes('bonus')) return 'gift-outline';
    if (name.includes('đầu tư') || name.includes('invest')) return 'trending-up-outline';
    return isExpense ? 'pricetag-outline' : 'wallet-outline';
  };

  const iconColor = item.color_code || (isExpense ? COLORS.expense : COLORS.income);

  return (
    <View style={styles.card}>
      {/* 1. Icon tròn đại diện */}
      <View style={[styles.iconWrapper, { backgroundColor: iconColor + '20' }]}>
        <Ionicons name={getCategoryIcon(item.category_name)} size={22} color={iconColor} />
      </View>

      {/* 2. Nội dung chính: Tên danh mục, Ghi chú, Ngày */}
      <View style={styles.centerInfo}>
        <Text style={styles.categoryTitle} numberOfLines={1}>
          {item.category_name || (isExpense ? 'Khoản Chi' : 'Khoản Thu')}
        </Text>
        <Text style={styles.noteText} numberOfLines={1}>
          {item.note || 'Không có ghi chú'}
        </Text>
        <View style={styles.metaRow}>
          <Text style={styles.dateText}>{item.transaction_date}</Text>
          {item.wallet_name && (
            <Text style={styles.walletBadge}>{item.wallet_name}</Text>
          )}
        </View>
      </View>

      {/* 3. Số tiền và nút xóa */}
      <View style={styles.rightInfo}>
        <Text style={[styles.amountText, isExpense ? styles.expenseText : styles.incomeText]}>
          {isExpense ? '-' : '+'}
          {amount.toLocaleString('vi-VN')} đ
        </Text>

        {onDelete && (
          <TouchableOpacity
            style={styles.deleteTouch}
            onPress={() => onDelete(item)}
            hitSlop={{ top: 8, bottom: 8, left: 8, right: 8 }}
          >
            <Ionicons name="trash-outline" size={16} color={COLORS.danger} />
            <Text style={styles.deleteLabel}>Xóa</Text>
          </TouchableOpacity>
        )}
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: COLORS.card,
    borderRadius: SIZES.radiusMd,
    padding: SIZES.md,
    marginBottom: SIZES.sm,
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: COLORS.border,
    elevation: 1,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 1
  },
  iconWrapper: {
    width: 44,
    height: 44,
    borderRadius: 22,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: SIZES.md
  },
  centerInfo: {
    flex: 1,
    justifyContent: 'center'
  },
  categoryTitle: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.text,
    marginBottom: 2
  },
  noteText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginBottom: 4
  },
  metaRow: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  dateText: {
    fontSize: SIZES.fontSm - 2,
    color: COLORS.textSecondary,
    marginRight: SIZES.sm
  },
  walletBadge: {
    fontSize: SIZES.fontSm - 3,
    color: COLORS.primaryDark,
    backgroundColor: COLORS.primaryLight,
    paddingHorizontal: 6,
    paddingVertical: 1,
    borderRadius: SIZES.radiusSm,
    fontWeight: '600'
  },
  rightInfo: {
    alignItems: 'flex-end',
    justifyContent: 'center',
    marginLeft: SIZES.sm
  },
  amountText: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    marginBottom: 4
  },
  expenseText: {
    color: COLORS.expense
  },
  incomeText: {
    color: COLORS.income
  },
  deleteTouch: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 2
  },
  deleteLabel: {
    fontSize: SIZES.fontSm - 2,
    color: COLORS.danger,
    marginLeft: 2,
    fontWeight: '500'
  }
});
