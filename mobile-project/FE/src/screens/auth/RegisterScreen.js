import React, { useState } from 'react';
import { View, Text, StyleSheet, Alert, TouchableOpacity } from 'react-native';
import { useAuth } from '../../context/AuthContext';
import { COLORS, SPACING, SIZES } from '../../constants/theme';

/**
 * ==============================================================================
 * MÀN HÌNH: ĐĂNG KÝ TÀI KHOẢN (REGISTER SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Đã tích hợp sẵn 100% bên dưới
 * ==============================================================================
 * 
 * 📌 CÁC BIẾN & HÀM LOGIC CÓ SẴN ĐỂ ĐẠI DÙNG:
 * 1. State:
 *    - `fullName`, `setFullName`: Lưu họ tên người dùng
 *    - `email`, `setEmail`: Lưu địa chỉ email
 *    - `password`, `setPassword`: Lưu mật khẩu
 *    - `confirmPassword`, `setConfirmPassword`: Lưu mật khẩu nhập lại
 *    - `isLoading`: Trạng thái đang gọi API
 * 
 * 2. Hàm xử lý (Handler):
 *    - `handleRegister()`: Kiểm tra hợp lệ và gọi API đăng ký (Backend tự cấp ngay 2 ví cơ bản và JWT token)
 * 
 * 3. Điều hướng:
 *    - `navigation.navigate('Login')`: Quay về màn hình Đăng nhập
 * ==============================================================================
 */

export default function RegisterScreen({ navigation }) {
  const { register, isLoading } = useAuth();
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  // Hàm xử lý đăng ký tài khoản mới
  const handleRegister = async () => {
    if (!fullName.trim() || !email.trim() || !password.trim()) {
      Alert.alert('Thông báo', 'Vui lòng điền đầy đủ thông tin bắt buộc.');
      return;
    }

    if (password.length < 6) {
      Alert.alert('Lỗi', 'Mật khẩu phải từ 6 ký tự trở lên.');
      return;
    }

    if (password !== confirmPassword) {
      Alert.alert('Lỗi', 'Mật khẩu xác nhận không khớp.');
      return;
    }

    const result = await register(fullName.trim(), email.trim(), password);
    if (!result.success) {
      Alert.alert('Đăng ký thất bại', result.message || 'Không thể tạo tài khoản.');
    }
  };

  return (
    <View style={styles.container}>
      <View style={styles.placeholderBox}>
        <Text style={styles.title}>Màn hình Đăng Ký</Text>
        <Text style={styles.subtitle}>[Khu vực Trần Trọng Đại tự do thiết kế giao diện UI]</Text>
        <Text style={styles.hint}>
          Logic backend đã sẵn sàng: Gọi hàm handleRegister() để đăng ký tài khoản.
        </Text>

        <TouchableOpacity onPress={() => navigation.navigate('Login')} style={styles.tempBtn}>
          <Text style={styles.tempBtnText}>➜ Quay về Đăng nhập</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background,
    justifyContent: 'center',
    alignItems: 'center',
    padding: SPACING.lg
  },
  placeholderBox: {
    width: '100%',
    padding: SPACING.xl,
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusLg,
    borderWidth: 1,
    borderColor: COLORS.border,
    alignItems: 'center'
  },
  title: {
    fontSize: SIZES.fontXl,
    fontWeight: 'bold',
    color: COLORS.primaryDark,
    marginBottom: SPACING.xs
  },
  subtitle: {
    fontSize: SIZES.fontSm,
    color: COLORS.primary,
    fontWeight: '600',
    textAlign: 'center',
    marginBottom: SPACING.sm
  },
  hint: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    textAlign: 'center',
    marginBottom: SPACING.lg
  },
  tempBtn: {
    paddingTop: SPACING.md,
    borderTopWidth: 1,
    borderTopColor: COLORS.borderLight,
    width: '100%',
    alignItems: 'center'
  },
  tempBtnText: {
    fontSize: SIZES.fontSm,
    color: COLORS.primaryLight,
    fontWeight: 'bold'
  }
});
