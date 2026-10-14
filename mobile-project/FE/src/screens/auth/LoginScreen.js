import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TextInput,
  TouchableOpacity,
  Alert,
  ScrollView,
  ActivityIndicator
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useAuth } from '../../context/AuthContext';
import { COLORS, SPACING, SIZES, SHADOWS } from '../../constants/theme';
import CustomButton from '../../components/CustomButton';

/**
 * ==============================================================================
 * MÀN HÌNH: ĐĂNG NHẬP (LOGIN SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & JWT Auth: Trịnh Thành Đạt (Lead Architect)
 * ==============================================================================
 */
export default function LoginScreen({ navigation }) {
  const { login, isLoading } = useAuth();
  const [email, setEmail] = useState('thanhdat@gmail.com');
  const [password, setPassword] = useState('password123');
  const [localLoading, setLocalLoading] = useState(false);

  const handleLogin = async () => {
    if (!email.trim() || !password.trim()) {
      Alert.alert('Thông báo', 'Vui lòng nhập đầy đủ Email và Mật khẩu.');
      return;
    }

    setLocalLoading(true);
    try {
      const result = await login(email.trim(), password);
      if (!result.success) {
        Alert.alert('Đăng nhập thất bại', result.message || 'Email hoặc mật khẩu không chính xác.');
      }
    } catch (error) {
      Alert.alert('Lỗi', error.message);
    } finally {
      setLocalLoading(false);
    }
  };

  const handleBypass = async () => {
    // Thử đăng nhập tài khoản seed hoặc thông báo
    await login('thanhdat@gmail.com', 'password123');
  };

  const loadingState = isLoading || localLoading;

  return (
    <ScrollView contentContainerStyle={styles.scrollContainer} keyboardShouldPersistTaps="handled">
      {/* 1. Header Thương hiệu App */}
      <View style={styles.header}>
        <View style={styles.iconCircle}>
          <Ionicons name="wallet" size={38} color={COLORS.primary} />
        </View>
        <Text style={styles.appTitle}>EXPENSE TRACKER</Text>
        <Text style={styles.subtitle}>Quản Lý Tài Chính Cá Nhân Thông Minh</Text>
      </View>

      {/* 2. Thẻ Form Đăng Nhập */}
      <View style={styles.formCard}>
        <Text style={styles.label}>Email Đăng Nhập</Text>
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

        <Text style={[styles.label, { marginTop: SPACING.md }]}>Mật Khẩu</Text>
        <View style={styles.inputWrap}>
          <Ionicons name="lock-closed-outline" size={18} color={COLORS.textSecondary} style={{ marginRight: 8 }} />
          <TextInput
            style={styles.input}
            placeholder="Nhập mật khẩu"
            placeholderTextColor={COLORS.textMuted}
            value={password}
            onChangeText={setPassword}
            secureTextEntry
          />
        </View>

        <TouchableOpacity
          style={styles.forgotTouch}
          onPress={() => navigation.navigate('ForgotPassword')}
        >
          <Text style={styles.forgotText}>Quên mật khẩu?</Text>
        </TouchableOpacity>

        <CustomButton
          title="Đăng Nhập"
          onPress={handleLogin}
          loading={loadingState}
          type="primary"
          style={{ marginTop: SPACING.md }}
        />

        <TouchableOpacity
          style={styles.switchAuthTouch}
          onPress={() => navigation.navigate('Register')}
        >
          <Text style={styles.switchAuthText}>
            Chưa có tài khoản? <Text style={styles.highlightText}>Đăng ký ngay</Text>
          </Text>
        </TouchableOpacity>
      </View>

      {/* 3. Nút Bypass Test Nhanh */}
      <TouchableOpacity style={styles.bypassLink} onPress={handleBypass}>
        <Text style={styles.bypassText}>⚡ Đăng nhập nhanh tài khoản thử nghiệm</Text>
      </TouchableOpacity>
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
    width: 76,
    height: 76,
    borderRadius: 38,
    backgroundColor: COLORS.surfaceVariant,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: SPACING.sm,
    borderWidth: 1,
    borderColor: COLORS.border
  },
  appTitle: {
    fontSize: SIZES.fontXxl,
    fontWeight: 'bold',
    color: COLORS.primaryDark,
    textAlign: 'center',
    letterSpacing: 0.5
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
  forgotTouch: {
    alignSelf: 'flex-end',
    marginTop: SPACING.xs + 2,
    marginBottom: SPACING.xs
  },
  forgotText: {
    fontSize: SIZES.fontXs,
    color: COLORS.primary,
    fontWeight: '600'
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
  },
  bypassLink: {
    marginTop: SPACING.lg,
    alignItems: 'center'
  },
  bypassText: {
    color: COLORS.primaryLight,
    fontSize: SIZES.fontSm,
    fontWeight: '600'
  }
});
