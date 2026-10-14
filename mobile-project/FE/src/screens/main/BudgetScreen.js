import React, { useState, useEffect, useCallback } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ActivityIndicator,
  RefreshControl,
  ScrollView,
  TouchableOpacity,
  TextInput,
  Alert,
  Modal
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES, SHADOWS } from '../../constants/theme';
import BudgetBar from '../../components/BudgetBar';
import CustomButton from '../../components/CustomButton';

/**
 * ==============================================================================
 * MÀN HÌNH: HẠN MỨC NGÂN SÁCH (BUDGET SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Trịnh Thành Đạt (Lead Architect)
 * ==============================================================================
 */
export default function BudgetScreen() {
  const [budgetData, setBudgetData] = useState({
    month: 10,
    year: 2026,
    overall_budget: {
      limit_amount: 12000000,
      spent_amount: 8500000,
      remaining_amount: 3500000,
      percentage: 71,
      status_level: 'WARNING',
      status_text: 'Đã tiêu 71% ngân sách. Chú ý chi tiêu trong tầm an toàn!',
      days_remaining: 24,
      daily_safe_spending: 145000
    },
    category_budgets: [
      { id: 1, name: 'Ăn uống', limit_amount: 5000000, spent_amount: 3800000, percentage: 76, color_code: '#FF6B6B' },
      { id: 2, name: 'Đi lại', limit_amount: 1500000, spent_amount: 900000, percentage: 60, color_code: '#FFE66D' },
      { id: 3, name: 'Mua sắm', limit_amount: 2500000, spent_amount: 2200000, percentage: 88, color_code: '#4ECDC4' },
      { id: 4, name: 'Hóa đơn', limit_amount: 2000000, spent_amount: 1600000, percentage: 80, color_code: '#1A535C' }
    ]
  });

  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [newLimitInput, setNewLimitInput] = useState('');
  const [saving, setSaving] = useState(false);

  const fetchBudgetStatus = async () => {
    try {
      const res = await api.getBudgetStatus();
      if (res.data) {
        setBudgetData(res.data);
      }
    } catch (error) {
      console.warn('[BudgetScreen] Lỗi nạp ngân sách, dùng fallback mock:', error.message);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    fetchBudgetStatus();
  }, []);

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    fetchBudgetStatus();
  }, []);

  const handleOpenEdit = () => {
    const currentLimit = budgetData?.overall_budget?.limit_amount || 12000000;
    setNewLimitInput(currentLimit.toString());
    setShowEditModal(true);
  };

  const handleSaveBudget = async () => {
    const numericLimit = parseFloat(newLimitInput.replace(/[^0-9]/g, ''));
    if (!numericLimit || numericLimit <= 0) {
      Alert.alert('Lỗi', 'Vui lòng nhập hạn mức lớn hơn 0 đ.');
      return;
    }

    setSaving(true);
    try {
      await api.setBudget({
        categoryId: null,
        amountLimit: numericLimit,
        month: budgetData?.month || new Date().getMonth() + 1,
        year: budgetData?.year || new Date().getFullYear()
      });
      Alert.alert('Thành công', 'Đã cập nhật hạn mức ngân sách tháng mới!');
      setShowEditModal(false);
      fetchBudgetStatus();
    } catch (error) {
      // Mock update local
      setBudgetData((prev) => ({
        ...prev,
        overall_budget: {
          ...prev.overall_budget,
          limit_amount: numericLimit,
          remaining_amount: Math.max(0, numericLimit - (prev.overall_budget?.spent_amount || 0)),
          percentage: Math.round(((prev.overall_budget?.spent_amount || 0) / numericLimit) * 100)
        }
      }));
      Alert.alert('Thành công (Chế độ xem trước)', 'Đã cập nhật hạn mức ngân sách tháng!');
      setShowEditModal(false);
    } finally {
      setSaving(false);
    }
  };

  const overall = budgetData?.overall_budget;
  const rawLimit = overall?.limit_amount ? parseFloat(overall.limit_amount) : 0;
  const isBudgetSet = rawLimit > 0;
  const percentage = isBudgetSet ? (overall?.percentage || 0) : 0;
  const isDanger = isBudgetSet && percentage >= 100;
  const isWarning = isBudgetSet && percentage >= 80 && percentage < 100;

  // Màu sắc và Icon cảnh báo đồng bộ chuẩn xác
  const statusColor = !isBudgetSet
    ? '#0284C7' // Xanh lam thông tin khi chưa đặt ngân sách
    : isDanger
      ? COLORS.danger
      : isWarning
        ? COLORS.warning
        : COLORS.income;

  const statusIcon = !isBudgetSet
    ? 'information-circle'
    : isDanger
      ? 'alert-circle'
      : isWarning
        ? 'warning'
        : 'checkmark-circle';

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color={COLORS.primary} />
        <Text style={styles.loadingText}>Đang nạp tiến độ ngân sách...</Text>
      </View>
    );
  }

  return (
    <ScrollView
      style={styles.container}
      contentContainerStyle={styles.contentContainer}
      refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={[COLORS.primary]} />}
    >
      {/* 1. Thẻ Ngân Sách Tổng Quát */}
      <View style={styles.mainCard}>
        <BudgetBar
          spent={overall?.spent_amount || 0}
          limit={rawLimit}
          month={`Tháng ${budgetData?.month || 10}/${budgetData?.year || 2026}`}
          isCard={false}
        />

        {/* Thanh Thông Điệp Cảnh Báo Thông Minh */}
        <View style={[styles.alertBox, { backgroundColor: statusColor + '15', borderColor: statusColor }]}>
          <Ionicons
            name={statusIcon}
            size={22}
            color={statusColor}
            style={{ marginRight: 8 }}
          />
          <Text style={[styles.alertText, { color: statusColor }]}>
            {overall?.status_text || (isBudgetSet ? 'Ngân sách trong tầm kiểm soát!' : 'Chưa thiết lập ngân sách tổng cho tháng này.')}
          </Text>
        </View>

        {/* Nút Chỉnh Sửa Hạn Mức */}
        <TouchableOpacity style={styles.editLimitBtn} onPress={handleOpenEdit}>
          <Ionicons name="settings-outline" size={18} color={COLORS.primary} style={{ marginRight: 6 }} />
          <Text style={styles.editLimitText}>Thiết Lập Hạn Mức Tháng Mới</Text>
        </TouchableOpacity>
      </View>

      {/* 2. Các Chỉ Số Chi Tiêu An Toàn Mỗi Ngày */}
      <View style={styles.metricGrid}>
        <View style={styles.metricCard}>
          <View style={styles.metricIconWrap}>
            <Ionicons name="speedometer-outline" size={20} color={COLORS.primary} />
          </View>
          <Text style={styles.metricLabel}>Chi an toàn / ngày</Text>
          <Text style={styles.metricValue}>
            {(overall?.daily_safe_spending || 0).toLocaleString('vi-VN')} đ
          </Text>
          <Text style={styles.metricSub}>Mức tối đa khuyên nghị</Text>
        </View>

        <View style={styles.metricCard}>
          <View style={[styles.metricIconWrap, { backgroundColor: '#E0F2FE' }]}>
            <Ionicons name="hourglass-outline" size={20} color="#0284C7" />
          </View>
          <Text style={styles.metricLabel}>Số ngày còn lại</Text>
          <Text style={[styles.metricValue, { color: '#0284C7' }]}>
            {overall?.days_remaining || 24} ngày
          </Text>
          <Text style={styles.metricSub}>Đến hết kỳ ngân sách</Text>
        </View>
      </View>

      {/* 3. Phân Rã Ngân Sách Theo Danh Mục (Nếu có) */}
      <View style={styles.sectionHeaderRow}>
        <Text style={styles.sectionTitle}>Ngân Sách Theo Danh Mục</Text>
        <Text style={styles.sectionSubtitle}>Phân loại chi tiết</Text>
      </View>

      {(budgetData?.category_budgets || []).map((cat) => {
        const catSpent = parseFloat(cat.spent_amount || 0);
        const catLimit = parseFloat(cat.limit_amount || 0);
        const catPct = catLimit > 0 ? Math.round((catSpent / catLimit) * 100) : 0;
        const catColor = cat.color_code || COLORS.primary;

        return (
          <View key={cat.id} style={styles.catBudgetCard}>
            <View style={styles.catHeader}>
              <View style={styles.catTitleRow}>
                <View style={[styles.catDot, { backgroundColor: catColor }]} />
                <Text style={styles.catName}>{cat.name}</Text>
              </View>
              <Text style={[styles.catPercent, { color: catColor }]}>
                {catLimit > 0 ? `${catPct}%` : 'Chưa đặt'}
              </Text>
            </View>

            <View style={styles.catProgressTrack}>
              <View
                style={[
                  styles.catProgressFill,
                  { width: `${catLimit > 0 ? Math.min(catPct, 100) : 0}%`, backgroundColor: catColor }
                ]}
              />
            </View>

            <View style={styles.catFooter}>
              <Text style={styles.catFooterText}>
                Đã tiêu: <Text style={{ fontWeight: 'bold' }}>{catSpent.toLocaleString('vi-VN')} đ</Text>
              </Text>
              <Text style={styles.catFooterText}>
                Hạn mức: {catLimit > 0 ? `${catLimit.toLocaleString('vi-VN')} đ` : 'Chưa thiết lập'}
              </Text>
            </View>
          </View>
        );
      })}

      {/* Modal Chỉnh Sửa Hạn Mức */}
      <Modal visible={showEditModal} transparent animationType="fade">
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={styles.modalTitle}>Cập Nhật Hạn Mức Ngân Sách</Text>
            <Text style={styles.modalSub}>
              Nhập số tiền tối đa bạn muốn chi tiêu cho Tháng {budgetData?.month}/{budgetData?.year}:
            </Text>

            <TextInput
              style={styles.modalInput}
              keyboardType="numeric"
              placeholder="Ví dụ: 15000000"
              placeholderTextColor={COLORS.textMuted}
              value={newLimitInput}
              onChangeText={setNewLimitInput}
            />

            <View style={styles.modalBtnRow}>
              <TouchableOpacity
                style={styles.modalCancelBtn}
                onPress={() => setShowEditModal(false)}
              >
                <Text style={styles.modalCancelText}>Hủy Bỏ</Text>
              </TouchableOpacity>

              <TouchableOpacity
                style={styles.modalSaveBtn}
                onPress={handleSaveBudget}
                disabled={saving}
              >
                {saving ? (
                  <ActivityIndicator size="small" color={COLORS.textWhite} />
                ) : (
                  <Text style={styles.modalSaveText}>Lưu Thay Đổi</Text>
                )}
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
  mainCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusLg,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    overflow: 'hidden',
    ...SHADOWS.card
  },
  alertBox: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: SPACING.sm + 4,
    borderRadius: SIZES.radiusSm,
    borderWidth: 1,
    marginTop: SPACING.md
  },
  alertText: {
    flex: 1,
    fontSize: SIZES.fontSm,
    fontWeight: '600'
  },
  editLimitBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: SPACING.md,
    paddingVertical: SPACING.sm,
    backgroundColor: COLORS.surfaceVariant,
    borderRadius: SIZES.radiusSm
  },
  editLimitText: {
    fontSize: SIZES.fontSm,
    color: COLORS.primary,
    fontWeight: 'bold'
  },
  metricGrid: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: SPACING.md
  },
  metricCard: {
    width: '48%',
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: COLORS.border,
    ...SHADOWS.small
  },
  metricIconWrap: {
    width: 36,
    height: 36,
    borderRadius: 18,
    backgroundColor: COLORS.surfaceVariant,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: SPACING.xs
  },
  metricLabel: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    marginBottom: 2
  },
  metricValue: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.textPrimary
  },
  metricSub: {
    fontSize: SIZES.fontXs - 2,
    color: COLORS.textMuted,
    marginTop: 2
  },
  sectionHeaderRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: SPACING.sm,
    marginTop: SPACING.xs
  },
  sectionTitle: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.textPrimary
  },
  sectionSubtitle: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary
  },
  catBudgetCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.sm,
    ...SHADOWS.small
  },
  catHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: SPACING.xs
  },
  catTitleRow: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  catDot: {
    width: 10,
    height: 10,
    borderRadius: 5,
    marginRight: 8
  },
  catName: {
    fontSize: SIZES.fontSm,
    fontWeight: '600',
    color: COLORS.textPrimary
  },
  catPercent: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold'
  },
  catProgressTrack: {
    height: 8,
    backgroundColor: COLORS.surfaceVariant,
    borderRadius: 4,
    overflow: 'hidden',
    marginVertical: 4
  },
  catProgressFill: {
    height: '100%',
    borderRadius: 4
  },
  catFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: 4
  },
  catFooterText: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary
  },
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.5)',
    justifyContent: 'center',
    padding: SPACING.lg
  },
  modalContent: {
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusLg,
    padding: SPACING.lg
  },
  modalTitle: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.primaryDark,
    marginBottom: SPACING.xs
  },
  modalSub: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginBottom: SPACING.md
  },
  modalInput: {
    borderWidth: 1,
    borderColor: COLORS.border,
    borderRadius: SIZES.radiusSm,
    padding: SPACING.md,
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    backgroundColor: COLORS.surfaceVariant,
    color: COLORS.textPrimary,
    marginBottom: SPACING.lg
  },
  modalBtnRow: {
    flexDirection: 'row',
    justifyContent: 'flex-end'
  },
  modalCancelBtn: {
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm,
    marginRight: SPACING.sm
  },
  modalCancelText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    fontWeight: '600'
  },
  modalSaveBtn: {
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
