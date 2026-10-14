import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TextInput,
  TouchableOpacity,
  Alert,
  ScrollView
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useAuth } from '../../context/AuthContext';
import { COLORS, SPACING, SIZES, SHADOWS } from '../../constants/theme';
import CustomButton from '../../components/CustomButton';

/**
 * ==============================================================================
 * MÀN HÌNH: ĐĂNG KÝ TÀI KHOẢN (REGISTER SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & JWT Auth: Trịnh Thành Đạt (Lead Architect)
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
            placeholder="Ví dụ: Trần Trọng Đại"
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

const styles = StyleSheet.create({
  scrollContainer: {
    flexGrow: 1,
    backgroundColor: COLORS.background,
    justifyContent: 'center',
    padding: SPACING.lg
  },
  header: {
    alignItems: 'center',
    marginBottom: SPACING.lg
  },
  iconCircle: {
    width: 68,
    height: 68,
    borderRadius: 34,
    backgroundColor: COLORS.surfaceVariant,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: SPACING.sm,
    borderWidth: 1,
    borderColor: COLORS.border
  },
  appTitle: {
    fontSize: SIZES.fontXl,
    fontWeight: 'bold',
    color: COLORS.primaryDark,
    textAlign: 'center'
  },
  subtitle: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    textAlign: 'center',
    marginTop: 2
  },
  formCard: {
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusLg,
    padding: SPACING.lg,
    borderWidth: 1,
    borderColor: COLORS.border,
    ...SHADOWS.card
  },
  label: {
    fontSize: SIZES.fontSm,
    fontWeight: '600',
    color: COLORS.textPrimary,
    marginBottom: 6
  },
  inputWrap: {
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: COLORS.border,
    borderRadius: SIZES.radiusSm,
    paddingHorizontal: SPACING.md,
    backgroundColor: COLORS.surfaceVariant
  },
  input: {
    flex: 1,
    paddingVertical: SPACING.sm + 2,
    fontSize: SIZES.fontSm,
    color: COLORS.textPrimary
  },
  switchAuthTouch: {
    marginTop: SPACING.md,
    alignItems: 'center',
    paddingVertical: SPACING.xs
  },
  switchAuthText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  highlightText: {
    color: COLORS.primary,
    fontWeight: 'bold'
  }
});
