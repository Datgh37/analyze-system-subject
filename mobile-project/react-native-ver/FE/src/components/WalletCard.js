import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { COLORS, SIZES } from '../constants/theme';

/**
 * LINH KIỆN THẺ VÍ TÀI CHÍNH (WALLET CARD)
 * Sinh viên thực hiện: Trần Trọng Đại (UI Specialist)
 * Phục vụ: Hiển thị thẻ ví tiền mặt / ngân hàng trên HomeScreen và AddTransactionScreen
 */
export default function WalletCard({ wallet, isSelected, onPress }) {
  const isBank = wallet.wallet_type === 'BANK' || (wallet.name && wallet.name.toLowerCase().includes('ngân hàng'));
  const balance = typeof wallet.balance === 'number' ? wallet.balance : parseFloat(wallet.balance || 0);

  return (
    <TouchableOpacity
      activeOpacity={0.8}
      onPress={onPress}
      style={[
        styles.card,
        isSelected && styles.cardSelected,
        isBank ? styles.cardBank : styles.cardCash
      ]}
    >
      <View style={styles.headerRow}>
        <View style={[styles.iconCircle, isBank ? styles.iconCircleBank : styles.iconCircleCash]}>
          <Ionicons
            name={isBank ? 'card-outline' : 'cash-outline'}
            size={22}
            color={isBank ? '#1565C0' : COLORS.primary}
          />
        </View>
        <Text style={styles.badgeText}>{isBank ? 'Tài Khoản' : 'Tiền Mặt'}</Text>
      </View>

      <Text style={styles.walletName} numberOfLines={1}>
        {wallet.name || 'Ví của tôi'}
      </Text>

      <Text style={styles.balanceText}>
        {balance.toLocaleString('vi-VN')} đ
      </Text>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  card: {
    width: 170,
    borderRadius: SIZES.radiusMd,
    padding: SIZES.md,
    marginRight: SIZES.sm,
    backgroundColor: COLORS.card,
    borderWidth: 1.5,
    borderColor: COLORS.border,
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.1,
    shadowRadius: 2
  },
  cardCash: {
    borderLeftWidth: 4,
    borderLeftColor: COLORS.primary
  },
  cardBank: {
    borderLeftWidth: 4,
    borderLeftColor: '#1976D2'
  },
  cardSelected: {
    borderColor: COLORS.primary,
    backgroundColor: COLORS.primaryLight
  },
  headerRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: SIZES.sm
  },
  iconCircle: {
    width: 36,
    height: 36,
    borderRadius: 18,
    justifyContent: 'center',
    alignItems: 'center'
  },
  iconCircleCash: {
    backgroundColor: '#E8F5E9'
  },
  iconCircleBank: {
    backgroundColor: '#E3F2FD'
  },
  badgeText: {
    fontSize: SIZES.fontSm - 2,
    fontWeight: 'bold',
    color: COLORS.textSecondary,
    textTransform: 'uppercase'
  },
  walletName: {
    fontSize: SIZES.fontMd,
    fontWeight: '600',
    color: COLORS.text,
    marginBottom: 4
  },
  balanceText: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.text
  }
});
