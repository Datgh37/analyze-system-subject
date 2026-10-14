import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  Alert,
  ScrollView
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useAuth } from '../../context/AuthContext';
import { authStyles as styles, COLORS, SPACING } from '../../constants/appStyles';
import CustomButton from '../../components/CustomButton';

/**
 * ==============================================================================
 * MÀN HÌNH: ĐĂNG KÝ TÀI KHOẢN (REGISTER SCREEN)
 * Quản lý Chi tiêu & Hoạch định Tài chính Cá nhân Thông minh
 * ==============================================================================
 */
export default function RegisterScreen({ navigation }) {
  const { register, isLoading } = useAuth();
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [localLoading, setLocalLoading] = useState(false);

  const handleRegister = async () => {
    if (!fullName.trim() || !email.trim() || !password) {
      Alert.alert('Thông báo', 'Vui lòng điền đầy đủ các thông tin bắt buộc.');
      return;
    }

    if (password.length < 6) {
      Alert.alert('Thông báo', 'Mật khẩu phải có độ dài tối thiểu 6 ký tự.');
      return;
    }

    if (password !== confirmPassword) {
      Alert.alert('Thông báo', 'Mật khẩu xác nhận không khớp. Vui lòng kiểm tra lại.');
      return;
    }

    setLocalLoading(true);
    try {
      const result = await register(fullName.trim(), email.trim(), password);
      if (!result.success) {
        Alert.alert('Đăng ký thất bại', result.message || 'Không thể tạo tài khoản.');
      } else {
        Alert.alert('Thành công', 'Đăng ký tài khoản thành công!');
      }
    } catch (error) {
      Alert.alert('Lỗi', error.message);
    } finally {
      setLocalLoading(false);
    }
  };

  const loadingState = isLoading || localLoading;

  return (
    <ScrollView contentContainerStyle={styles.scrollContainer} keyboardShouldPersistTaps="handled">
      {/* 1. Header */}
      <View style={styles.header}>
        <View style={styles.iconCircle}>
          <Ionicons name="person-add-outline" size={32} color={COLORS.primary} />
        </View>
        <Text style={styles.appTitle}>TẠO TÀI KHOẢN MỚI</Text>
        <Text style={styles.subtitle}>Bắt đầu quản lý tài chính cá nhân khoa học</Text>
      </View>

      {/* 2. Thẻ Form Đăng Ký */}
      <View style={styles.formCard}>
        <Text style={styles.label}>Họ và Tên</Text>
        <View style={styles.inputWrap}>
          <Ionicons name="person-outline" size={18} color={COLORS.textSecondary} style={{ marginRight: 8 }} />
          <TextInput
            style={styles.input}
            placeholder="Ví dụ: Nguyễn Văn An"
            placeholderTextColor={COLORS.textMuted}
            value={fullName}
            onChangeText={setFullName}
          />
        </View>

        <Text style={[styles.label, { marginTop: SPACING.md }]}>Email</Text>
        <View style={styles.inputWrap}>
          <Ionicons name="mail-outline" size={18} color={COLORS.textSecondary} style={{ marginRight: 8 }} />
          <TextInput
            style={styles.input}
            placeholder="email@example.com"
            placeholderTextColor={COLORS.textMuted}
            value={email}
            onChangeText={setEmail}
            autoCapitalize="none"
            keyboardType="email-address"
          />
        </View>

        <Text style={[styles.label, { marginTop: SPACING.md }]}>Mật Khẩu (Tối thiểu 6 ký tự)</Text>
        <View style={styles.inputWrap}>
          <Ionicons name="lock-closed-outline" size={18} color={COLORS.textSecondary} style={{ marginRight: 8 }} />
          <TextInput
            style={styles.input}
            placeholder="Nhập mật khẩu an toàn"
            placeholderTextColor={COLORS.textMuted}
            value={password}
            onChangeText={setPassword}
            secureTextEntry
          />
        </View>

        <Text style={[styles.label, { marginTop: SPACING.md }]}>Xác Nhận Mật Khẩu</Text>
        <View style={styles.inputWrap}>
          <Ionicons name="shield-checkmark-outline" size={18} color={COLORS.textSecondary} style={{ marginRight: 8 }} />
          <TextInput
            style={styles.input}
            placeholder="Nhập lại mật khẩu"
            placeholderTextColor={COLORS.textMuted}
            value={confirmPassword}
            onChangeText={setConfirmPassword}
            secureTextEntry
          />
        </View>

        <CustomButton
          title="Đăng Ký Tài Khoản"
          onPress={handleRegister}
          loading={loadingState}
          type="primary"
          style={{ marginTop: SPACING.lg }}
        />

        <TouchableOpacity
          style={styles.switchAuthTouch}
          onPress={() => navigation.navigate('Login')}
        >
          <Text style={styles.switchAuthText}>
            Đã có tài khoản? <Text style={styles.highlightText}>Đăng nhập ngay</Text>
          </Text>
        </TouchableOpacity>
      </View>
    </ScrollView>
  );
}
