import React from 'react';
import { TouchableOpacity, Text, ActivityIndicator } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { COLORS, customButtonStyles as styles } from '../constants/appStyles';

/**
 * LINH KIỆN NÚT BẤM DÙNG CHUNG (CUSTOM BUTTON)
 * Sinh viên thực hiện: Trần Trọng Đại (UI Specialist)
 * Styling tập trung quản lý tại FE/src/constants/appStyles.js
 */
export default function CustomButton({
  title,
  onPress,
  type = 'primary',
  loading = false,
  icon = null,
  style = {}
}) {
  const getButtonStyle = () => {
    switch (type) {
      case 'outline':
        return styles.btnOutline;
      case 'danger':
        return styles.btnDanger;
      case 'income':
        return styles.btnIncome;
      default:
        return styles.btnPrimary;
    }
  };

  const getTextStyle = () => {
    switch (type) {
      case 'outline':
        return styles.textOutline;
      default:
        return styles.textWhite;
    }
  };

  return (
    <TouchableOpacity
      activeOpacity={0.8}
      style={[styles.baseBtn, getButtonStyle(), style]}
      onPress={onPress}
      disabled={loading}
    >
      {loading ? (
        <ActivityIndicator color={type === 'outline' ? COLORS.primary : COLORS.white} size="small" />
      ) : (
        <>
          {icon && (
            <Ionicons
              name={icon}
              size={18}
              color={type === 'outline' ? COLORS.primary : COLORS.white}
              style={{ marginRight: 8 }}
            />
          )}
          <Text style={[styles.baseText, getTextStyle()]}>{title}</Text>
        </>
      )}
    </TouchableOpacity>
  );
}

