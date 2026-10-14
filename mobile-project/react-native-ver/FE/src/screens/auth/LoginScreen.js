import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  Alert,
  ScrollView,
  ActivityIndicator
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useAuth } from '../../context/AuthContext';
import { authStyles as styles, COLORS, SPACING } from '../../constants/appStyles';
import CustomButton from '../../components/CustomButton';

/**
 * ==============================================================================
 * MÀN HÌNH: ĐĂNG NHẬP (LOGIN SCREEN)
 * Quản lý Chi tiêu & Hoạch định Tài chính Cá nhân Thông minh
 * ==============================================================================
 */
export default function LoginScreen({ navigation }) {
  const { login, isLoading } = useAuth();
  const [email, setEmail] = useState('nguyenvanan@gmail.com');
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
    await login('nguyenvanan@gmail.com', 'password123');
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

      {/* 3. Nút Đăng nhập nhanh trải nghiệm */}
      <TouchableOpacity style={styles.bypassLink} onPress={handleBypass}>
        <Text style={styles.bypassText}>Đăng nhập nhanh</Text>
      </TouchableOpacity>
    </ScrollView>
  );
}
