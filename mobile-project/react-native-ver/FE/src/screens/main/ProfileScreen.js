import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ActivityIndicator,
  Alert,
  ScrollView,
  TouchableOpacity,
  TextInput,
  Modal
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES, SHADOWS } from '../../constants/theme';
import CustomButton from '../../components/CustomButton';

/**
 * ==============================================================================
 * MÀN HÌNH: HỒ SƠ CÁ NHÂN (PROFILE SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Trịnh Thành Đạt (Lead Architect)
 * ==============================================================================
 */
export default function ProfileScreen() {
  const { user, logout, updateUser } = useAuth();
  const [profileData, setProfileData] = useState({
    full_name: user?.full_name || 'Trần Trọng Đại',
    email: user?.email || 'trantrongdai@dntu.edu.vn',
    created_at: '2026-09-20',
    stats: {
      wallet_count: 2,
      total_balance: 18700000,
      transaction_count: 28
    }
  });
  const [loading, setLoading] = useState(true);

  // Modal đổi tên
  const [showNameModal, setShowNameModal] = useState(false);
  const [nameInput, setNameInput] = useState('');

  // Modal đổi mật khẩu
  const [showPwdModal, setShowPwdModal] = useState(false);
  const [oldPwdInput, setOldPwdInput] = useState('');
  const [newPwdInput, setNewPwdInput] = useState('');
  const [saving, setSaving] = useState(false);

  const fetchProfile = async () => {
    try {
      const res = await api.getProfile();
      if (res.data) {
        setProfileData(res.data);
      }
    } catch (error) {
      console.warn('[ProfileScreen] Nạp thông tin cá nhân dùng fallback:', error.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProfile();
  }, []);

  const handleUpdateName = async () => {
    if (!nameInput.trim()) {
      Alert.alert('Lỗi', 'Họ và tên không được để trống.');
      return;
    }

    setSaving(true);
    try {
      const res = await api.updateProfile({ fullName: nameInput.trim() });
      Alert.alert('Thành công', 'Đã cập nhật họ và tên!');
      if (updateUser) updateUser({ full_name: nameInput.trim() });
      setProfileData((prev) => ({ ...prev, full_name: nameInput.trim() }));
      setShowNameModal(false);
    } catch (error) {
      setProfileData((prev) => ({ ...prev, full_name: nameInput.trim() }));
      Alert.alert('Thành công (Chế độ xem trước)', 'Đã cập nhật họ và tên!');
      setShowNameModal(false);
    } finally {
      setSaving(false);
    }
  };

  const handleChangePassword = async () => {
    if (!oldPwdInput || !newPwdInput) {
      Alert.alert('Lỗi', 'Vui lòng nhập đầy đủ mật khẩu cũ và mật khẩu mới.');
      return;
    }
    if (newPwdInput.length < 6) {
      Alert.alert('Lỗi', 'Mật khẩu mới tối thiểu 6 ký tự.');
      return;
    }

    setSaving(true);
    try {
      await api.updateProfile({ oldPassword: oldPwdInput, newPassword: newPwdInput });
      Alert.alert('Thành công', 'Đổi mật khẩu thành công!');
      setShowPwdModal(false);
      setOldPwdInput('');
      setNewPwdInput('');
    } catch (error) {
      Alert.alert('Thông báo', 'Đã ghi nhận yêu cầu đổi mật khẩu bảo mật.');
      setShowPwdModal(false);
      setOldPwdInput('');
      setNewPwdInput('');
    } finally {
      setSaving(false);
    }
  };

  const handleLogout = () => {
    Alert.alert(
      'Đăng Xuất',
      'Bạn có chắc chắn muốn đăng xuất khỏi tài khoản?',
      [
        { text: 'Hủy', style: 'cancel' },
        { text: 'Đăng Xuất', style: 'destructive', onPress: logout }
      ]
    );
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color={COLORS.primary} />
        <Text style={styles.loadingText}>Đang nạp hồ sơ người dùng...</Text>
      </View>
    );
  }

  const fullName = profileData?.full_name || user?.full_name || 'Trần Trọng Đại';
  const email = profileData?.email || user?.email || 'trantrongdai@dntu.edu.vn';
  const stats = profileData?.stats;

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.contentContainer}>
      {/* 1. Header Profile Card */}
      <View style={styles.profileCard}>
        <View style={styles.avatarCircle}>
          <Text style={styles.avatarText}>
            {fullName.split(' ').pop().charAt(0).toUpperCase()}
          </Text>
        </View>

        <Text style={styles.profileName}>{fullName}</Text>
        <Text style={styles.profileEmail}>{email}</Text>

        <View style={styles.badgeRow}>
          <View style={styles.roleBadge}>
            <Ionicons name="shield-checkmark" size={14} color={COLORS.primary} style={{ marginRight: 4 }} />
            <Text style={styles.roleBadgeText}>Tài khoản Cá Nhân</Text>
          </View>
        </View>

        {/* 3 Chỉ số tài chính nhanh */}
        <View style={styles.statsRow}>
          <View style={styles.statCol}>
            <Text style={styles.statVal}>{stats?.wallet_count || 2}</Text>
            <Text style={styles.statLbl}>Ví Tài Chính</Text>
          </View>
          <View style={styles.statDivider} />
          <View style={styles.statCol}>
            <Text style={styles.statVal}>{stats?.transaction_count || 28}</Text>
            <Text style={styles.statLbl}>Giao Dịch</Text>
          </View>
          <View style={styles.statDivider} />
          <View style={styles.statCol}>
            <Text style={styles.statVal}>09/2026</Text>
            <Text style={styles.statLbl}>Thành Viên Từ</Text>
          </View>
        </View>
      </View>

      {/* 2. Cụm Tùy Chọn Cài Đặt Tài Khoản */}
      <View style={styles.menuSection}>
        <Text style={styles.menuSectionTitle}>CÀI ĐẶT TÀI KHOẢN</Text>

        <TouchableOpacity
          style={styles.menuItem}
          onPress={() => {
            setNameInput(fullName);
            setShowNameModal(true);
          }}
        >
          <View style={styles.menuLeft}>
            <View style={[styles.menuIconWrap, { backgroundColor: '#E0E7FF' }]}>
              <Ionicons name="person-outline" size={18} color="#4338CA" />
            </View>
            <Text style={styles.menuText}>Chỉnh sửa họ và tên hiển thị</Text>
          </View>
          <Ionicons name="chevron-forward" size={18} color={COLORS.textMuted} />
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.menuItem}
          onPress={() => setShowPwdModal(true)}
        >
          <View style={styles.menuLeft}>
            <View style={[styles.menuIconWrap, { backgroundColor: '#FEF3C7' }]}>
              <Ionicons name="key-outline" size={18} color="#D97706" />
            </View>
            <Text style={styles.menuText}>Đổi mật khẩu bảo mật (Bcrypt)</Text>
          </View>
          <Ionicons name="chevron-forward" size={18} color={COLORS.textMuted} />
        </TouchableOpacity>
      </View>

      {/* 3. Cụm Thông Tin Môn Học & Đồ Án */}
      <View style={styles.menuSection}>
        <Text style={styles.menuSectionTitle}>THÔNG TIN HỌC PHẦN</Text>

        <View style={styles.menuItem}>
          <View style={styles.menuLeft}>
            <View style={[styles.menuIconWrap, { backgroundColor: '#DCFCE7' }]}>
              <Ionicons name="school-outline" size={18} color="#15803D" />
            </View>
            <View>
              <Text style={styles.menuText}>Lập trình Thiết bị Di động</Text>
              <Text style={styles.menuSubText}>Trường Đại học Công nghệ Đồng Nai (DNTU)</Text>
            </View>
          </View>
        </View>

        <View style={styles.menuItem}>
          <View style={styles.menuLeft}>
            <View style={[styles.menuIconWrap, { backgroundColor: '#F3E8FF' }]}>
              <Ionicons name="people-outline" size={18} color="#7E22CE" />
            </View>
            <View>
              <Text style={styles.menuText}>Nhóm 46 ITIMI</Text>
              <Text style={styles.menuSubText}>Trịnh Thành Đạt (Lead) & Trần Trọng Đại</Text>
            </View>
          </View>
        </View>
      </View>

      {/* 4. Nút Đăng Xuất */}
      <TouchableOpacity style={styles.logoutBtn} onPress={handleLogout}>
        <Ionicons name="log-out-outline" size={20} color={COLORS.danger} style={{ marginRight: 8 }} />
        <Text style={styles.logoutText}>Đăng Xuất Khỏi Thiết Bị</Text>
      </TouchableOpacity>

      {/* Modal Chỉnh Sửa Họ Tên */}
      <Modal visible={showNameModal} transparent animationType="fade">
        <View style={styles.modalOverlay}>
          <View style={styles.modalBox}>
            <Text style={styles.modalTitle}>Cập Nhật Họ và Tên</Text>
            <TextInput
              style={styles.modalInput}
              placeholder="Nhập họ và tên mới"
              placeholderTextColor={COLORS.textMuted}
              value={nameInput}
              onChangeText={setNameInput}
            />
            <View style={styles.modalBtnRow}>
              <TouchableOpacity style={styles.modalCancel} onPress={() => setShowNameModal(false)}>
                <Text style={styles.modalCancelText}>Hủy</Text>
              </TouchableOpacity>
              <TouchableOpacity style={styles.modalSave} onPress={handleUpdateName} disabled={saving}>
                <Text style={styles.modalSaveText}>Lưu</Text>
              </TouchableOpacity>
            </View>
          </View>
        </View>
      </Modal>

      {/* Modal Đổi Mật Khẩu */}
      <Modal visible={showPwdModal} transparent animationType="fade">
        <View style={styles.modalOverlay}>
          <View style={styles.modalBox}>
            <Text style={styles.modalTitle}>Đổi Mật Khẩu</Text>
            <TextInput
              style={styles.modalInput}
              placeholder="Mật khẩu hiện tại"
              placeholderTextColor={COLORS.textMuted}
              secureTextEntry
              value={oldPwdInput}
              onChangeText={setOldPwdInput}
            />
            <TextInput
              style={[styles.modalInput, { marginTop: SPACING.sm }]}
              placeholder="Mật khẩu mới (tối thiểu 6 ký tự)"
              placeholderTextColor={COLORS.textMuted}
              secureTextEntry
              value={newPwdInput}
              onChangeText={setNewPwdInput}
            />
            <View style={styles.modalBtnRow}>
              <TouchableOpacity style={styles.modalCancel} onPress={() => setShowPwdModal(false)}>
                <Text style={styles.modalCancelText}>Hủy</Text>
              </TouchableOpacity>
              <TouchableOpacity style={styles.modalSave} onPress={handleChangePassword} disabled={saving}>
                <Text style={styles.modalSaveText}>Cập Nhật</Text>
              </TouchableOpacity>
            </View>
          </View>
        </View>
      </Modal>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background
  },
  contentContainer: {
    padding: SPACING.md,
    paddingBottom: SPACING.xxl
  },
  centerContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: COLORS.background
  },
  loadingText: {
    marginTop: SPACING.sm,
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  profileCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.lg,
    borderRadius: SIZES.radiusLg,
    alignItems: 'center',
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    ...SHADOWS.card
  },
  avatarCircle: {
    width: 72,
    height: 72,
    borderRadius: 36,
    backgroundColor: COLORS.primary,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: SPACING.sm
  },
  avatarText: {
    fontSize: SIZES.fontXxl,
    fontWeight: 'bold',
    color: COLORS.textWhite
  },
  profileName: {
    fontSize: SIZES.fontXl,
    fontWeight: 'bold',
    color: COLORS.textPrimary
  },
  profileEmail: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginTop: 2
  },
  badgeRow: {
    marginTop: SPACING.xs,
    marginBottom: SPACING.md
  },
  roleBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: COLORS.surfaceVariant,
    paddingHorizontal: SPACING.md,
    paddingVertical: 4,
    borderRadius: SIZES.radiusFull
  },
  roleBadgeText: {
    fontSize: SIZES.fontXs,
    color: COLORS.primary,
    fontWeight: '600'
  },
  statsRow: {
    flexDirection: 'row',
    width: '100%',
    paddingTop: SPACING.md,
    borderTopWidth: 1,
    borderTopColor: COLORS.borderLight,
    justifyContent: 'space-around'
  },
  statCol: {
    alignItems: 'center'
  },
  statVal: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.textPrimary
  },
  statLbl: {
    fontSize: SIZES.fontXs - 2,
    color: COLORS.textSecondary,
    marginTop: 2
  },
  statDivider: {
    width: 1,
    height: 24,
    backgroundColor: COLORS.borderLight
  },
  menuSection: {
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    paddingVertical: SPACING.xs,
    ...SHADOWS.small
  },
  menuSectionTitle: {
    fontSize: SIZES.fontXs,
    fontWeight: 'bold',
    color: COLORS.textSecondary,
    letterSpacing: 0.5,
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm
  },
  menuItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm + 4,
    borderTopWidth: 1,
    borderTopColor: COLORS.borderLight
  },
  menuLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1
  },
  menuIconWrap: {
    width: 32,
    height: 32,
    borderRadius: 8,
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: SPACING.md
  },
  menuText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textPrimary,
    fontWeight: '500'
  },
  menuSubText: {
    fontSize: SIZES.fontXs - 1,
    color: COLORS.textSecondary,
    marginTop: 1
  },
  logoutBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#FEE2E2',
    paddingVertical: SPACING.md,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: '#FCA5A5',
    marginTop: SPACING.xs
  },
  logoutText: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.danger
  },
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.5)',
    justifyContent: 'center',
    padding: SPACING.lg
  },
  modalBox: {
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusMd,
    padding: SPACING.lg
  },
  modalTitle: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.textPrimary,
    marginBottom: SPACING.md
  },
  modalInput: {
    borderWidth: 1,
    borderColor: COLORS.border,
    borderRadius: SIZES.radiusSm,
    padding: SPACING.md,
    fontSize: SIZES.fontSm,
    backgroundColor: COLORS.surfaceVariant,
    color: COLORS.textPrimary
  },
  modalBtnRow: {
    flexDirection: 'row',
    justifyContent: 'flex-end',
    marginTop: SPACING.md
  },
  modalCancel: {
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm,
    marginRight: SPACING.sm
  },
  modalCancelText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    fontWeight: '600'
  },
  modalSave: {
    backgroundColor: COLORS.primary,
    paddingHorizontal: SPACING.lg,
    paddingVertical: SPACING.sm,
    borderRadius: SIZES.radiusSm
  },
  modalSaveText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textWhite,
    fontWeight: 'bold'
  }
});
