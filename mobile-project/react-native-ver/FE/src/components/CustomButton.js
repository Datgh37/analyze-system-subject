import React from 'react';
import { TouchableOpacity, Text, StyleSheet, ActivityIndicator } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { COLORS, SIZES } from '../constants/theme';

/**
 * LINH KIỆN NÚT BẤM DÙNG CHUNG (CUSTOM BUTTON)
 * Sinh viên thực hiện: Trần Trọng Đại (UI Specialist)
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

const styles = StyleSheet.create({
  baseBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: SIZES.md - 2,
    paddingHorizontal: SIZES.lg,
    borderRadius: SIZES.radiusMd,
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.1,
    shadowRadius: 2
  },
  btnPrimary: {
    backgroundColor: COLORS.primary
  },
  btnDanger: {
    backgroundColor: COLORS.danger
  },
  btnIncome: {
    backgroundColor: COLORS.income
  },
  btnOutline: {
    backgroundColor: 'transparent',
    borderWidth: 1.5,
    borderColor: COLORS.primary,
    elevation: 0
  },
  baseText: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold'
  },
  textWhite: {
    color: COLORS.white
  },
  textOutline: {
    color: COLORS.primary
  }
});
