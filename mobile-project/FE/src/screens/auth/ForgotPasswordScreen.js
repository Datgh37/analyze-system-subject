import React, { useState } from 'react';
import { View, Text, StyleSheet, Alert, TouchableOpacity } from 'react-native';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES } from '../../constants/theme';

/**
 * ==============================================================================
 * MÀN HÌNH: QUÊN MẬT KHẨU 3 BƯỚC OTP (FORGOT PASSWORD SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Đã tích hợp sẵn 100% bên dưới
 * ==============================================================================
 * 
 * 📌 CÁC BIẾN & HÀM LOGIC CÓ SẴN ĐỂ ĐẠI DÙNG:
 * 1. State quản lý:
 *    - `step`: Bước hiện tại (1: Nhập Email, 2: Nhập OTP, 3: Đặt mật khẩu mới)
 *    - `email`, `setEmail`: Email tài khoản
 *    - `otpCode`, `setOtpCode`: Mã OTP 6 chữ số
 *    - `resetToken`: Token xác thực tạm thời cấp sau bước 2
 *    - `newPassword`, `setNewPassword`: Mật khẩu mới
 *    - `loading`: Trạng thái đang gửi API
 * 
 * 2. Các hàm xử lý 3 bước (Handlers):
 *    - `handleRequestOtp()`: Bước 1 - Gửi email yêu cầu OTP (tự động chuyển sang bước 2)
 *    - `handleVerifyOtp()`: Bước 2 - Xác thực mã OTP 6 số (tự động chuyển sang bước 3)
 *    - `handleResetPassword()`: Bước 3 - Đặt lại mật khẩu mới (xong chuyển về Login)
 * ==============================================================================
 */

export default function ForgotPasswordScreen({ navigation }) {
  const [step, setStep] = useState(1);
  const [email, setEmail] = useState('');
  const [otpCode, setOtpCode] = useState('');
  const [resetToken, setResetToken] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);

  // Bước 1: Yêu cầu OTP
  const handleRequestOtp = async () => {
    if (!email.trim()) {
      Alert.alert('Lỗi', 'Vui lòng nhập email tài khoản của bạn.');
      return;
    }

    setLoading(true);
    try {
      const res = await api.forgotPassword(email.trim());
      Alert.alert('Thành công', 'Mã OTP đã được tạo. Vui lòng nhập mã để tiếp tục.');
      if (res.data?.debug_otp) {
        setOtpCode(res.data.debug_otp); // Hỗ trợ test nhanh trên Localhost
      }
      setStep(2);
    } catch (err) {
      Alert.alert('Lỗi', err.message);
    } finally {
      setLoading(false);
    }
  };

  // Bước 2: Xác thực OTP
  const handleVerifyOtp = async () => {
    if (!otpCode.trim() || otpCode.trim().length !== 6) {
      Alert.alert('Lỗi', 'Mã OTP phải gồm 6 chữ số.');
      return;
    }

    setLoading(true);
    try {
      const res = await api.verifyOtp(email.trim(), otpCode.trim());
      setResetToken(res.data.reset_token);
      setStep(3);
    } catch (err) {
      Alert.alert('Xác thực thất bại', err.message);
    } finally {
      setLoading(false);
    }
  };

  // Bước 3: Đặt lại Mật khẩu mới
  const handleResetPassword = async () => {
    if (newPassword.length < 6) {
      Alert.alert('Lỗi', 'Mật khẩu mới phải có tối thiểu 6 ký tự.');
      return;
    }

    if (newPassword !== confirmPassword) {
      Alert.alert('Lỗi', 'Mật khẩu xác nhận không trùng khớp.');
      return;
    }

    setLoading(true);
    try {
      await api.resetPassword(email.trim(), resetToken, newPassword);
      Alert.alert(
        'Đổi mật khẩu thành công!',
        'Bạn có thể đăng nhập ngay bằng mật khẩu mới.',
        [{ text: 'Đăng nhập ngay', onPress: () => navigation.navigate('Login') }]
      );
    } catch (err) {
      Alert.alert('Lỗi', err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <View style={styles.placeholderBox}>
        <Text style={styles.title}>Màn hình Quên Mật Khẩu (OTP)</Text>
        <Text style={styles.subtitle}>[Khu vực Trần Trọng Đại tự do thiết kế giao diện UI]</Text>
        <Text style={styles.hint}>
          Đã cài sẵn quy trình 3 bước: handleRequestOtp() ➜ handleVerifyOtp() ➜ handleResetPassword().
        </Text>
        <Text style={styles.stepInfo}>Bước hiện tại trong logic: {step}/3</Text>

        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.tempBtn}>
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
    marginBottom: SPACING.sm
  },
  stepInfo: {
    fontSize: SIZES.fontSm,
    color: COLORS.warning,
    fontWeight: 'bold',
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
