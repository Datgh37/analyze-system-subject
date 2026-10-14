import React from 'react';
import { View, Text, TouchableOpacity } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { transactionItemStyles as styles } from '../constants/appStyles';

/**
 * LINH KIỆN DÒNG GIAO DỊCH THU / CHI (TRANSACTION ITEM)
 * Thiết kế chuẩn Modern Fintech: Bố cục rộng rãi, chữ số rõ nét, không bị đè chữ.
 * Styling tập trung quản lý tại FE/src/constants/appStyles.js
 */
export default function TransactionItem({ item, onPress, onDelete }) {
  const isExpense = item.transaction_type === 'EXPENSE';
  const amount = typeof item.amount === 'number' ? item.amount : parseFloat(item.amount || 0);

  // Ánh xạ icon danh mục
  const getCategoryIcon = (categoryName) => {
    const name = (categoryName || '').toLowerCase();
    if (name.includes('ăn') || name.includes('uống') || name.includes('cà phê') || name.includes('food')) return 'restaurant-outline';
    if (name.includes('xe') || name.includes('xăng') || name.includes('di chuyển')) return 'car-sport-outline';
    if (name.includes('mua') || name.includes('sắm') || name.includes('tiêu dùng')) return 'cart-outline';
    if (name.includes('nhà') || name.includes('phòng')) return 'home-outline';
    if (name.includes('hóa đơn') || name.includes('điện') || name.includes('nước')) return 'receipt-outline';
    if (name.includes('lương') || name.includes('salary')) return 'cash-outline';
    if (name.includes('thưởng') || name.includes('năng suất')) return 'gift-outline';
    if (name.includes('đầu tư') || name.includes('tiết kiệm')) return 'trending-up-outline';
    return isExpense ? 'pricetag-outline' : 'wallet-outline';
  };

  // Định dạng ngày hiển thị thân thiện (DD/MM/YYYY)
  const formatDate = (dateStr) => {
    if (!dateStr) return '';
    try {
      const parts = dateStr.split('-');
      if (parts.length === 3) {
        return `${parts[2]}/${parts[1]}/${parts[0]}`;
      }
      return dateStr;
    } catch {
      return dateStr;
    }
  };

  const iconColor = item.color_code || (isExpense ? '#EF4444' : '#10B981');

  return (
    <View style={styles.card}>
      <TouchableOpacity
        style={styles.mainContent}
        activeOpacity={0.7}
        onPress={onPress}
        disabled={!onPress}
      >
        {/* 1. Icon Squircle hiện đại */}
        <View style={[styles.iconWrapper, { backgroundColor: iconColor + '15' }]}>
          <Ionicons name={getCategoryIcon(item.category_name)} size={22} color={iconColor} />
        </View>

        {/* 2. Thông tin giao dịch trung tâm */}
        <View style={styles.centerInfo}>
          <Text style={styles.categoryTitle} numberOfLines={1}>
            {item.category_name || (isExpense ? 'Khoản Chi' : 'Khoản Thu')}
          </Text>
          <Text style={styles.noteText} numberOfLines={1}>
            {item.note || 'Không có ghi chú'}
          </Text>
          <View style={styles.metaRow}>
            <Text style={styles.dateText}>{formatDate(item.transaction_date)}</Text>
            {item.wallet_name ? (
              <View style={styles.walletBadgeContainer}>
                <Text style={styles.walletBadge} numberOfLines={1}>{item.wallet_name}</Text>
              </View>
            ) : null}
          </View>
        </View>

        {/* 3. Cột số tiền rõ ràng */}
        <View style={styles.amountCol}>
          <Text style={[styles.amountText, isExpense ? styles.expenseText : styles.incomeText]}>
            {isExpense ? '-' : '+'}
            {amount.toLocaleString('vi-VN')} đ
          </Text>
        </View>
      </TouchableOpacity>

      {/* 4. Nút xóa độc lập bên phải */}
      {onDelete && (
        <TouchableOpacity
          style={styles.deleteButton}
          onPress={() => onDelete(item)}
          activeOpacity={0.6}
          hitSlop={{ top: 10, bottom: 10, left: 6, right: 6 }}
        >
          <Ionicons name="trash-outline" size={17} color="#EF4444" />
        </TouchableOpacity>
      )}
    </View>
  );
}

