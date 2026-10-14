import React, { useState } from 'react';
import {
  View,
  Text,
  Alert,
  TouchableOpacity,
  TextInput,
  ScrollView,
  ActivityIndicator
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { api } from '../../api/endpoints';
import { authStyles as styles, COLORS, SPACING } from '../../constants/appStyles';
import CustomButton from '../../components/CustomButton';

/**
 * ==============================================================================
 * MÀN HÌNH: QUÊN MẬT KHẨU 3 BƯỚC OTP (FORGOT PASSWORD SCREEN)
 * Quy trình Wizard 3 bước bảo mật chuẩn Fintech
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
  const [debugOtpHint, setDebugOtpHint] = useState('');

  // Bước 1: Yêu cầu mã OTP qua Email
  const handleRequestOtp = async () => {
    if (!email.trim()) {
      Alert.alert('Thông báo', 'Vui lòng nhập địa chỉ email tài khoản của bạn.');
      return;
    }

    setLoading(true);
    try {
      const res = await api.forgotPassword(email.trim());
      Alert.alert('Thành công', 'Mã xác thực OTP đã được gửi. Vui lòng kiểm tra và nhập mã.');
      if (res.data?.debug_otp) {
        setOtpCode(res.data.debug_otp); // Điền trước cho test
        setDebugOtpHint(res.data.debug_otp);
      }
      setStep(2);
    } catch (err) {
      Alert.alert('Lỗi gửi OTP', err.message);
    } finally {
      setLoading(false);
    }
  };

  // Bước 2: Xác thực mã OTP 6 số
  const handleVerifyOtp = async () => {
    const cleanOtp = otpCode.trim();
    if (!cleanOtp || cleanOtp.length !== 6) {
      Alert.alert('Thông báo', 'Mã OTP phải gồm chính xác 6 chữ số.');
      return;
    }

    setLoading(true);
    try {
      const res = await api.verifyOtp(email.trim(), cleanOtp);
      setResetToken(res.data.reset_token);
      Alert.alert('Xác thực thành công', 'Mã OTP hợp lệ. Hãy tạo mật khẩu mới cho tài khoản.');
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
      Alert.alert('Thông báo', 'Mật khẩu mới phải có tối thiểu 6 ký tự.');
      return;
    }

    if (newPassword !== confirmPassword) {
      Alert.alert('Thông báo', 'Mật khẩu xác nhận không trùng khớp với mật khẩu mới.');
      return;
    }

    setLoading(true);
    try {
      await api.resetPassword(email.trim(), resetToken, newPassword);
      Alert.alert(
        'Đổi mật khẩu thành công!',
        'Mật khẩu tài khoản đã được cập nhật. Bạn có thể đăng nhập ngay.',
        [{ text: 'Đăng Nhập Ngay', onPress: () => navigation.navigate('Login') }]
      );
    } catch (err) {
      Alert.alert('Lỗi đặt mật khẩu', err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <ScrollView
      contentContainerStyle={styles.scrollContainer}
      keyboardShouldPersistTaps="handled"
    >
      {/* 1. Header & Thanh tiến trình 3 bước (Step Indicator) */}
      <View style={styles.header}>
        <View style={styles.iconCircle}>
          <Ionicons
            name={step === 1 ? 'mail-unread-outline' : step === 2 ? 'shield-checkmark-outline' : 'key-outline'}
            size={36}
            color={COLORS.primary}
          />
        </View>
        <Text style={styles.appTitle}>KHÔI PHỤC MẬT KHẨU</Text>
        <Text style={styles.subtitle}>Quy trình xác thực 3 bước bảo mật</Text>

        <View style={styles.stepProgressRow}>
          <View style={[styles.stepDot, step >= 1 && styles.stepDotActive]}>
            <Text style={[styles.stepDotText, step >= 1 && styles.stepDotTextActive]}>1</Text>
          </View>
          <View style={[styles.stepLine, step >= 2 && styles.stepLineActive]} />
          <View style={[styles.stepDot, step >= 2 && styles.stepDotActive]}>
            <Text style={[styles.stepDotText, step >= 2 && styles.stepDotTextActive]}>2</Text>
          </View>
          <View style={[styles.stepLine, step >= 3 && styles.stepLineActive]} />
          <View style={[styles.stepDot, step >= 3 && styles.stepDotActive]}>
            <Text style={[styles.stepDotText, step >= 3 && styles.stepDotTextActive]}>3</Text>
          </View>
        </View>

        <Text style={styles.stepTitle}>
          {step === 1 && 'Bước 1: Nhập Email Tài Khoản'}
          {step === 2 && 'Bước 2: Xác Thực Mã OTP'}
          {step === 3 && 'Bước 3: Tạo Mật Khẩu Mới'}
        </Text>
      </View>

      {/* 2. Thẻ Form Nội Dung Theo Từng Bước */}
      <View style={styles.formCard}>
        {/* ===================== BƯỚC 1 ===================== */}
        {step === 1 && (
          <View>
            <Text style={styles.stepDesc}>
              Nhập địa chỉ email bạn đã sử dụng để đăng ký tài khoản. Hệ thống sẽ cấp mã xác thực OTP 6 số.
            </Text>

            <Text style={styles.label}>Email Của Bạn</Text>
            <View style={styles.inputWrap}>
              <Ionicons name="mail-outline" size={18} color={COLORS.textSecondary} style={{ marginRight: 8 }} />
              <TextInput
                style={styles.input}
                placeholder="email@example.com"
                placeholderTextColor={COLORS.textMuted}
                value={email}
                onChangeText={setEmail}
                keyboardType="email-address"
                autoCapitalize="none"
              />
            </View>

            <CustomButton
              title="Gửi Mã Xác Nhận OTP"
              onPress={handleRequestOtp}
              loading={loading}
              type="primary"
              style={{ marginTop: SPACING.lg }}
            />
          </View>
        )}

        {/* ===================== BƯỚC 2 ===================== */}
        {step === 2 && (
          <View>
            <Text style={styles.stepDesc}>
              Mã xác thực gồm 6 chữ số đã được gửi tới <Text style={{ fontWeight: 'bold' }}>{email}</Text>. Vui lòng kiểm tra và nhập mã bên dưới:
            </Text>

            <Text style={styles.label}>Mã Xác Thực OTP (6 số)</Text>
            <View style={styles.inputWrap}>
              <Ionicons name="keypad-outline" size={18} color={COLORS.textSecondary} style={{ marginRight: 8 }} />
              <TextInput
                style={[styles.input, styles.otpInput]}
                placeholder="123456"
                placeholderTextColor={COLORS.textMuted}
                value={otpCode}
                onChangeText={setOtpCode}
                keyboardType="number-pad"
                maxLength={6}
              />
            </View>

            <CustomButton
              title="Xác Thực Mã OTP"
              onPress={handleVerifyOtp}
              loading={loading}
              type="primary"
              style={{ marginTop: SPACING.lg }}
            />

            <TouchableOpacity
              style={styles.subActionTouch}
              onPress={() => setStep(1)}
            >
              <Text style={styles.subActionText}>← Đổi địa chỉ email khác</Text>
            </TouchableOpacity>
          </View>
        )}

        {/* ===================== BƯỚC 3 ===================== */}
        {step === 3 && (
          <View>
            <Text style={styles.stepDesc}>
              Xác thực hoàn tất! Vui lòng thiết lập mật khẩu mới an toàn cho tài khoản.
            </Text>

            <Text style={styles.label}>Mật Khẩu Mới (Tối thiểu 6 ký tự)</Text>
            <View style={styles.inputWrap}>
              <Ionicons name="lock-closed-outline" size={18} color={COLORS.textSecondary} style={{ marginRight: 8 }} />
              <TextInput
                style={styles.input}
                placeholder="Nhập mật khẩu mới"
                placeholderTextColor={COLORS.textMuted}
                value={newPassword}
                onChangeText={setNewPassword}
                secureTextEntry
              />
            </View>

            <Text style={[styles.label, { marginTop: SPACING.md }]}>Xác Nhận Mật Khẩu Mới</Text>
            <View style={styles.inputWrap}>
              <Ionicons name="shield-checkmark-outline" size={18} color={COLORS.textSecondary} style={{ marginRight: 8 }} />
              <TextInput
                style={styles.input}
                placeholder="Nhập lại mật khẩu mới"
                placeholderTextColor={COLORS.textMuted}
                value={confirmPassword}
                onChangeText={setConfirmPassword}
                secureTextEntry
              />
            </View>

            <CustomButton
              title="Cập Nhật Mật Khẩu & Đăng Nhập"
              onPress={handleResetPassword}
              loading={loading}
              type="primary"
              style={{ marginTop: SPACING.lg }}
            />
          </View>
        )}

        {/* Nút quay lại đăng nhập */}
        <TouchableOpacity
          style={styles.backToLoginTouch}
          onPress={() => navigation.navigate('Login')}
        >
          <Text style={styles.backToLoginText}>
            Đã nhớ lại mật khẩu? <Text style={styles.backToLoginHighlight}>Quay về Đăng nhập</Text>
          </Text>
        </TouchableOpacity>
      </View>
    </ScrollView>
  );
}
