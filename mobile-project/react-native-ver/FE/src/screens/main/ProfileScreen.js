import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ActivityIndicator, Alert, ScrollView, TouchableOpacity } from 'react-native';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES } from '../../constants/theme';

/**
 * ==============================================================================
 * MÀN HÌNH: HỒ SƠ CÁ NHÂN (PROFILE SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Đã tích hợp sẵn 100% bên dưới
 * ==============================================================================
 * 
 * 📌 CÁC BIẾN & HÀM LOGIC CÓ SẴN ĐỂ ĐẠI DÙNG:
 * 1. State thông tin người dùng:
 *    - `profileData`:
 *      + `full_name`: Họ và tên hiển thị
 *      + `email`: Địa chỉ email
 *      + `created_at`: Ngày đăng ký
 *      + `stats.wallet_count`: Số lượng ví đang sở hữu
 *      + `stats.total_balance`: Tổng tài sản khả dụng
 *      + `stats.transaction_count`: Tổng số giao dịch đã thực hiện
 * 
 * 2. Hàm xử lý (Handlers):
 *    - `handleUpdateName(newName)`: Gọi API cập nhật họ tên mới
 *    - `handleChangePassword(oldPwd, newPwd)`: Gọi API đổi mật khẩu bảo mật Bcrypt
 *    - `handleLogout()`: Đăng xuất, xóa JWT token và chuyển về màn Login
 * ==============================================================================
 */

export default function ProfileScreen() {
  const { user, logout, updateUser } = useAuth();
  const [profileData, setProfileData] = useState(null);
  const [loading, setLoading] = useState(true);

  const fetchProfile = async () => {
    try {
      const res = await api.getProfile();
      if (res.data) {
        setProfileData(res.data);
      }
    } catch (error) {
      console.warn('[ProfileScreen] Lỗi nạp thông tin cá nhân:', error.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProfile();
  }, []);

  const handleUpdateName = async (newName) => {
    if (!newName || !newName.trim()) {
      Alert.alert('Lỗi', 'Họ và tên không được để trống.');
      return;
    }

    try {
      const res = await api.updateProfile({ fullName: newName.trim() });
      Alert.alert('Thành công', 'Đã cập nhật họ và tên!');
      updateUser({ full_name: res.data.full_name });
      fetchProfile();
    } catch (error) {
      Alert.alert('Lỗi', error.message);
    }
  };

  const handleChangePassword = async (oldPassword, newPassword) => {
    if (!oldPassword || !newPassword) {
      Alert.alert('Lỗi', 'Vui lòng nhập đầy đủ mật khẩu cũ và mới.');
      return;
    }

    try {
      await api.updateProfile({ oldPassword, newPassword });
      Alert.alert('Thành công', 'Đổi mật khẩu thành công!');
    } catch (error) {
      Alert.alert('Lỗi', error.message);
    }
  };

  const handleLogout = () => {
    Alert.alert(
      'Đăng Xuất',
      'Bạn có chắc chắn muốn đăng xuất khỏi ứng dụng?',
      [
        { text: 'Hủy', style: 'cancel' },
        { text: 'Đăng Xuất', style: 'destructive', onPress: logout }
      ]
    );
  };

  const formatCurrency = (amt) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amt || 0);
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color={COLORS.primary} />
      </View>
    );
  }

  const stats = profileData?.stats;

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.contentContainer}>
      <View style={styles.placeholderBox}>
        <Text style={styles.title}>Màn hình Hồ Sơ Cá Nhân</Text>
        <Text style={styles.subtitle}>[Khu vực Trần Trọng Đại tự do thiết kế giao diện UI]</Text>

        <View style={styles.dataPreviewBox}>
          <Text style={styles.dataPreviewTitle}>Dữ liệu hồ sơ người dùng:</Text>
          <Text style={styles.dataPreviewText}>• Họ và tên: {profileData?.full_name}</Text>
          <Text style={styles.dataPreviewText}>• Email: {profileData?.email}</Text>
          <Text style={styles.dataPreviewText}>• Ví tài chính: {stats?.wallet_count} ví</Text>
          <Text style={styles.dataPreviewText}>• Tổng tài sản: {formatCurrency(stats?.total_balance)}</Text>
          <Text style={styles.dataPreviewText}>• Số lượng giao dịch: {stats?.transaction_count} lần</Text>
        </View>

        <Text style={styles.hint}>
          Đại xem hướng dẫn trong file code: Các hàm `handleUpdateName()`, `handleChangePassword()` và `handleLogout()`.
        </Text>

        <TouchableOpacity onPress={handleLogout} style={styles.tempLogoutBtn}>
          <Text style={styles.tempLogoutText}>➜ Bấm để Đăng Xuất thử nghiệm</Text>
        </TouchableOpacity>
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background
  },
  contentContainer: {
    padding: SPACING.lg,
    flexGrow: 1,
    justifyContent: 'center'
  },
  centerContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center'
  },
  placeholderBox: {
    backgroundColor: COLORS.surface,
    padding: SPACING.xl,
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
    marginBottom: SPACING.lg
  },
  dataPreviewBox: {
    backgroundColor: COLORS.background,
    padding: SPACING.md,
    borderRadius: SIZES.radiusMd,
    width: '100%',
    marginBottom: SPACING.md
  },
  dataPreviewTitle: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold',
    color: COLORS.primaryDark,
    marginBottom: SPACING.xs
  },
  dataPreviewText: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    marginBottom: 2
  },
  hint: {
    fontSize: SIZES.fontXs,
    color: COLORS.textMuted,
    textAlign: 'center',
    marginBottom: SPACING.md
  },
  tempLogoutBtn: {
    paddingTop: SPACING.md,
    borderTopWidth: 1,
    borderTopColor: COLORS.borderLight,
    width: '100%',
    alignItems: 'center'
  },
  tempLogoutText: {
    fontSize: SIZES.fontSm,
    color: COLORS.danger,
    fontWeight: 'bold'
  }
});
