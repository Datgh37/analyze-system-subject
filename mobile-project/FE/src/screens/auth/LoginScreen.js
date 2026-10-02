import React, { useState } from 'react';
import { View, Text, StyleSheet, Alert, TouchableOpacity } from 'react-native';
import { useAuth } from '../../context/AuthContext';
import { COLORS, SPACING, SIZES } from '../../constants/theme';

/**
 * ==============================================================================
 * MÀN HÌNH: ĐĂNG NHẬP (LOGIN SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Đã tích hợp sẵn 100% bên dưới
 * ==============================================================================
 * 
 * 📌 CÁC BIẾN & HÀM LOGIC CÓ SẴN ĐỂ ĐẠI DÙNG:
 * 1. State:
 *    - `email`, `setEmail`: Lưu địa chỉ email
 *    - `password`, `setPassword`: Lưu mật khẩu
 *    - `isLoading`: Trạng thái đang gọi API (dùng cho ActivityIndicator)
 * 
 * 2. Hàm xử lý (Handler):
 *    - `handleLogin()`: Gọi API đăng nhập, tự động lưu JWT token vào máy và chuyển vào màn hình chính
 * 
 * 3. Điều hướng (Navigation):
 *    - `navigation.navigate('Register')`: Chuyển sang màn hình Đăng ký
 *    - `navigation.navigate('ForgotPassword')`: Chuyển sang màn hình Quên mật khẩu
 * ==============================================================================
 */

export default function LoginScreen({ navigation }) {
  const { login, isLoading } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  // Hàm xử lý đăng nhập - Nối trực tiếp vào API Backend
  const handleLogin = async () => {
    if (!email.trim() || !password.trim()) {
      Alert.alert('Thông báo', 'Vui lòng nhập đầy đủ Email và Mật khẩu.');
      return;
    }

    const result = await login(email.trim(), password);
    if (!result.success) {
      Alert.alert('Đăng nhập thất bại', result.message || 'Email hoặc mật khẩu không chính xác.');
    }
  };

  return (
    <View style={styles.container}>
      <View style={styles.placeholderBox}>
        <Text style={styles.title}>Màn hình Đăng Nhập</Text>
        <Text style={styles.subtitle}>[Khu vực Trần Trọng Đại tự do thiết kế giao diện UI]</Text>
        <Text style={styles.hint}>
          Logic backend đã sẵn sàng: Gọi hàm handleLogin() để đăng nhập.
        </Text>

        {/* Nút tạm thời để Đại bấm thử chuyển màn hình khi đang vẽ giao diện */}
        <View style={styles.tempNavRow}>
          <TouchableOpacity onPress={() => navigation.navigate('Register')}>
            <Text style={styles.tempLink}>➜ Sang Đăng ký</Text>
          </TouchableOpacity>
          <TouchableOpacity onPress={() => navigation.navigate('ForgotPassword')}>
            <Text style={styles.tempLink}>➜ Sang Quên mật khẩu</Text>
          </TouchableOpacity>
        </View>
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
  tempNavRow: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    width: '100%',
    paddingTop: SPACING.md,
    borderTopWidth: 1,
    borderTopColor: COLORS.borderLight
  },
  tempLink: {
    fontSize: SIZES.fontSm,
    color: COLORS.primaryLight,
    fontWeight: 'bold'
  }
});
