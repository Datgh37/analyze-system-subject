import React from 'react';
import { View, Text, TouchableOpacity } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { walletCardStyles as styles } from '../constants/appStyles';

/**
 * LINH KIỆN THẺ VÍ TÀI CHÍNH (WALLET CARD)
 * Thiết kế chuẩn Modern Fintech: Bố cục rộng rãi, không bị cụt chữ, nhận diện loại ví thông minh.
 * Styling tập trung quản lý tại FE/src/constants/appStyles.js
 */
export default function WalletCard({ wallet, isSelected, onPress }) {
  const nameLower = (wallet.name || '').toLowerCase();
  const isMoMo = nameLower.includes('momo');
  const isSavings = nameLower.includes('tiết kiệm') || nameLower.includes('dự phòng');
  const isBank = wallet.wallet_type === 'BANK' || nameLower.includes('ngân hàng') || nameLower.includes('vietcombank') || nameLower.includes('mb');
  
  const balance = typeof wallet.balance === 'number' ? wallet.balance : parseFloat(wallet.balance || 0);

  const getBadgeLabel = () => {
    if (isMoMo) return 'Ví MoMo';
    if (isSavings) return 'Tiết Kiệm';
    if (isBank) return 'Ngân Hàng';
    return 'Tiền Mặt';
  };

  const getIconName = () => {
    if (isMoMo) return 'phone-portrait-outline';
    if (isSavings) return 'shield-checkmark-outline';
    if (isBank) return 'card-outline';
    return 'cash-outline';
  };

  const getAccentColor = () => {
    if (isMoMo) return '#A21CAF'; // Tím hồng MoMo
    if (isSavings) return '#0D9488'; // Xanh ngọc tiết kiệm
    if (isBank) return '#2563EB'; // Xanh dương ngân hàng
    return '#16A34A'; // Xanh lá tiền mặt
  };

  const accentColor = getAccentColor();

  return (
    <TouchableOpacity
      activeOpacity={0.85}
      onPress={onPress}
      style={[
        styles.card,
        { borderLeftColor: accentColor },
        isSelected && styles.cardSelected
      ]}
    >
      <View style={styles.headerRow}>
        <View
          style={[
            styles.iconCircle,
            isSelected ? styles.iconCircleSelected : { backgroundColor: accentColor + '18' }
          ]}
        >
          <Ionicons
            name={getIconName()}
            size={18}
            color={isSelected ? '#FFFFFF' : accentColor}
          />
        </View>

        <View style={styles.badgeGroup}>
          <Text
            style={[
              styles.badgeText,
              { color: isSelected ? '#FFFFFF' : accentColor }
            ]}
          >
            {getBadgeLabel()}
          </Text>
          {isSelected && (
            <Ionicons
              name="checkmark-circle"
              size={15}
              color="#FFFFFF"
              style={{ marginLeft: 4 }}
            />
          )}
        </View>
      </View>

      <Text
        style={[
          styles.walletName,
          isSelected && styles.walletNameSelected
        ]}
        numberOfLines={1}
        ellipsizeMode="tail"
      >
        {wallet.name || 'Ví của tôi'}
      </Text>

      <Text
        style={[
          styles.balanceText,
          isSelected && styles.balanceTextSelected
        ]}
        numberOfLines={1}
      >
        {balance.toLocaleString('vi-VN')} đ
      </Text>
    </TouchableOpacity>
  );
}

