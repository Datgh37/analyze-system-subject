import React from 'react';
import { View, Text, StyleSheet } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { COLORS, SIZES } from '../constants/theme';

/**
 * LINH KIỆN THANH NGÂN SÁCH THÁNG ĐỔI MÀU (BUDGET BAR)
 * Sinh viên thực hiện: Trần Trọng Đại (UI Specialist)
 * Tiêu chí đồ án: Thanh tiến độ đổi màu Xanh -> Vàng -> Đỏ theo % chi tiêu thực tế
 */
export default function BudgetBar({
  spent = 0,
  limit = 0,
  month = '10/2026',
  isCard = true,
  style
}) {
  const safeSpent = typeof spent === 'number' ? spent : parseFloat(spent || 0);
  const safeLimit = typeof limit === 'number' ? limit : parseFloat(limit || 0);
  const isBudgetSet = safeLimit > 0;
  const percentage = isBudgetSet ? Math.round((safeSpent / safeLimit) * 100) : 0;
  const remaining = Math.max(0, safeLimit - safeSpent);

  // Chuẩn hóa tên tháng gọn gàng, tránh lặp chữ dài gây tràn layout
  const cleanMonth = month.includes('Tháng') ? month : `Tháng ${month}`;

  // Cơ chế đổi màu trực quan theo ngưỡng chi tiêu
  const getStatusColor = () => {
    if (!isBudgetSet) return COLORS.textSecondary; // Xám trung tính khi chưa đặt ngân sách
    if (percentage < 70) return COLORS.income;      // Xanh lá an toàn
    if (percentage <= 90) return COLORS.warning;    // Cam cảnh báo
    return COLORS.danger;                           // Đỏ nguy hiểm
  };

  const getStatusMessage = () => {
    if (!isBudgetSet) {
      return 'Chưa thiết lập hạn mức ngân sách tháng này. Hãy đặt hạn mức để kiểm soát chi tiêu!';
    }
    if (percentage < 70) return `Chi tiêu đang trong tầm kiểm soát (Còn ${remaining.toLocaleString('vi-VN')} đ)`;
    if (percentage <= 90) return `Cảnh báo: Đã tiêu ${percentage}% ngân sách! Hãy tiết kiệm hơn.`;
    return `Nguy hiểm: Đã tiêu ${percentage}% ngân sách tháng!`;
  };

  const statusColor = getStatusColor();

  return (
    <View style={[isCard ? styles.cardContainer : styles.plainContainer, style]}>
      <View style={styles.headerRow}>
        <View style={styles.titleGroup}>
          <Ionicons
            name={isBudgetSet ? 'pie-chart-outline' : 'calculator-outline'}
            size={18}
            color={statusColor}
            style={{ marginRight: 6 }}
          />
          <Text style={styles.title} numberOfLines={1} ellipsizeMode="tail">
            Ngân Sách {cleanMonth}
          </Text>
        </View>

        {/* Huy hiệu phần trăm dạng Pill gọn gàng, chống tràn 100% */}
        <View
          style={[
            styles.badgePill,
            { backgroundColor: isBudgetSet ? statusColor + '18' : '#F1F5F9' }
          ]}
        >
          <Text
            style={[
              styles.percentBadge,
              { color: isBudgetSet ? statusColor : COLORS.textSecondary }
            ]}
          >
            {isBudgetSet ? `${percentage}%` : 'Chưa đặt'}
          </Text>
        </View>
      </View>

      {/* Thanh tiến độ */}
      <View style={styles.track}>
        <View
          style={[
            styles.fill,
            {
              width: `${isBudgetSet ? Math.min(percentage, 100) : 0}%`,
              backgroundColor: statusColor
            }
          ]}
        />
      </View>

      {/* Thông số chi tiết */}
      <View style={styles.infoRow}>
        <Text style={styles.spentText}>
          Đã chi: <Text style={{ fontWeight: 'bold', color: COLORS.text }}>{safeSpent.toLocaleString('vi-VN')} đ</Text>
        </Text>
        <Text style={styles.limitText}>
          Hạn mức: <Text style={{ fontWeight: 'bold', color: isBudgetSet ? COLORS.textSecondary : COLORS.primary }}>
            {isBudgetSet ? `${safeLimit.toLocaleString('vi-VN')} đ` : 'Chưa thiết lập'}
          </Text>
        </Text>
      </View>

      {/* Dòng cảnh báo / lời khuyên */}
      <Text style={[styles.statusMsg, { color: statusColor }]}>
        {getStatusMessage()}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  cardContainer: {
    width: '100%',
    backgroundColor: COLORS.card,
    borderRadius: SIZES.radiusMd,
    padding: SIZES.md,
    borderWidth: 1,
    borderColor: COLORS.border,
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.08,
    shadowRadius: 2
  },
  plainContainer: {
    width: '100%',
    backgroundColor: 'transparent',
    padding: 0
  },
  headerRow: {
    width: '100%',
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: SIZES.sm
  },
  titleGroup: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
    marginRight: 8
  },
  title: {
    fontSize: 14,
    fontWeight: 'bold',
    color: COLORS.text,
    flex: 1
  },
  badgePill: {
    paddingHorizontal: 8,
    paddingVertical: 3,
    borderRadius: 12,
    alignItems: 'center',
    justifyContent: 'center',
    flexShrink: 0
  },
  percentBadge: {
    fontSize: 12,
    fontWeight: '700'
  },
  track: {
    width: '100%',
    height: 10,
    backgroundColor: '#EEEEEE',
    borderRadius: SIZES.radiusFull,
    overflow: 'hidden',
    marginBottom: SIZES.xs
  },
  fill: {
    height: '100%',
    borderRadius: SIZES.radiusFull
  },
  infoRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginTop: SIZES.xs
  },
  spentText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  limitText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  statusMsg: {
    fontSize: SIZES.fontSm - 1,
    fontWeight: '500',
    marginTop: 6
  }
});
