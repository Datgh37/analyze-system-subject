import React, { useState } from 'react';
import { View, Text, StyleSheet, TextInput, TouchableOpacity, Alert } from 'react-native';
import { COLORS, SIZES } from '../constants/theme';
import apiClient from '../api/client';

/**
 * MÀN HÌNH ĐĂNG NHẬP (LOGIN SCREEN)
 * Kiến trúc & Kết nối Auth: Trịnh Thành Đạt (Lead)
 * Phụ trách hoàn thiện UI & AsyncStorage: Trần Trọng Đại (UI Specialist)
 * 
 * 👉 Nhiệm vụ Đại add nốt:
 * - Lưu Token / User Info vào AsyncStorage để tự động đăng nhập
 * - Hoàn thiện màn hình Đăng ký tài khoản (RegisterScreen.js)
 */
export default function LoginScreen({ navigation }) {
  const [email, setEmail] = useState('thanhdat@gmail.com');
  const [password, setPassword] = useState('password123');

  const handleLogin = async () => {
    if (!email || !password) {
      Alert.alert('Thông báo', 'Vui lòng nhập đầy đủ Email và Mật khẩu.');
      return;
    }

    try {
      const res = await apiClient.post('/auth/login.php', { email, password });
      if (res && res.status === 'success') {
        Alert.alert('Đăng nhập thành công', `Chào mừng ${res.data.user.full_name}!`, [
          { text: 'Tiếp tục', onPress: () => navigation.replace('MainTab') }
        ]);
      }
    } catch (err) {
      Alert.alert('Đăng nhập thất bại', err.message);
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.appTitle}>EXPENSE TRACKER</Text>
      <Text style={styles.subtitle}>Quản lý Chi tiêu Cá nhân Thông minh</Text>

      <View style={styles.formCard}>
        <Text style={styles.label}>Email Đăng Nhập</Text>
        <TextInput
          style={styles.input}
          placeholder="email@example.com"
          value={email}
          onChangeText={setEmail}
          autoCapitalize="none"
          keyboardType="email-address"
        />

        <Text style={[styles.label, { marginTop: SIZES.md }]}>Mật Khẩu</Text>
        <TextInput
          style={styles.input}
          placeholder="Nhập mật khẩu"
          value={password}
          onChangeText={setPassword}
          secureTextEntry
        />

        <TouchableOpacity style={styles.loginButton} onPress={handleLogin}>
          <Text style={styles.loginButtonText}>Đăng Nhập</Text>
        </TouchableOpacity>
      </View>

      {/* Nút vào thẳng xem app (Bypass để test nhanh) */}
      <TouchableOpacity
        style={styles.bypassLink}
        onPress={() => navigation.replace('MainTab')}
      >
        <Text style={styles.bypassText}>Bỏ qua đăng nhập để test giao diện ➔</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background,
    justifyContent: 'center',
    padding: SIZES.lg
  },
  appTitle: {
    fontSize: SIZES.fontTitle + 4,
    fontWeight: 'bold',
    color: COLORS.primary,
    textAlign: 'center'
  },
  subtitle: {
    fontSize: SIZES.fontMd,
    color: COLORS.textSecondary,
    textAlign: 'center',
    marginTop: SIZES.xs,
    marginBottom: SIZES.xl
  },
  formCard: {
    backgroundColor: COLORS.card,
    borderRadius: SIZES.radiusMd,
    padding: SIZES.lg,
    elevation: 3
  },
  label: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold',
    color: COLORS.text,
    marginBottom: SIZES.xs
  },
  input: {
    borderWidth: 1,
    borderColor: COLORS.border,
    borderRadius: SIZES.radiusSm,
    padding: SIZES.sm,
    fontSize: SIZES.fontMd,
    color: COLORS.text
  },
  loginButton: {
    backgroundColor: COLORS.primary,
    paddingVertical: SIZES.md,
    borderRadius: SIZES.radiusSm,
    alignItems: 'center',
    marginTop: SIZES.lg
  },
  loginButtonText: {
    color: COLORS.white,
    fontSize: SIZES.fontMd,
    fontWeight: 'bold'
  },
  bypassLink: {
    marginTop: SIZES.lg,
    alignItems: 'center'
  },
  bypassText: {
    color: COLORS.primary,
    fontSize: SIZES.fontSm,
    textDecorationLine: 'underline'
  }
});
