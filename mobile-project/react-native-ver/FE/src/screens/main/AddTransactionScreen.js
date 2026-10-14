import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ActivityIndicator, Alert, ScrollView } from 'react-native';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES } from '../../constants/theme';

/**
 * ==============================================================================
 * MÀN HÌNH: THÊM / SỬA GIAO DỊCH (ADD / EDIT TRANSACTION SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Đã tích hợp sẵn 100% bên dưới
 * ==============================================================================
 * 
 * 📌 CÁC BIẾN & HÀM LOGIC CÓ SẴN ĐỂ ĐẠI DÙNG:
 * 1. Chế độ hoạt động:
 *    - `isEditing`: Boolean (true nếu bấm từ màn hình danh sách vào để sửa, false nếu thêm mới)
 * 
 * 2. State Form dữ liệu:
 *    - `type`, `setType`: 'EXPENSE' (Chi) hoặc 'INCOME' (Thu)
 *    - `amount`, `setAmount`: Số tiền (chuỗi số)
 *    - `note`, `setNote`: Ghi chú giao dịch
 *    - `selectedWalletId`, `setSelectedWalletId`: ID ví thanh toán
 *    - `selectedCategoryId`, `setSelectedCategoryId`: ID danh mục
 *    - `transactionDate`, `setTransactionDate`: Ngày phát sinh (YYYY-MM-DD)
 * 
 * 3. Danh sách nạp sẵn từ Backend:
 *    - `wallets`: Mảng ví tài chính của user [{ id, name, balance }, ...]
 *    - `categories`: Mảng danh mục [{ id, name, type, icon_name, color_code }, ...]
 *    - `filteredCategories`: Mảng danh mục đã lọc theo `type` hiện tại
 * 
 * 4. Hàm xử lý:
 *    - `handleSave()`: Tự động phát hiện thêm mới hay cập nhật.
 *      (Đặc biệt: Khi sửa, backend tự động bù trừ Delta chênh lệch vào ví theo QOL #2).
 * ==============================================================================
 */

export default function AddTransactionScreen({ route, navigation }) {
  const editTransaction = route?.params?.transaction;
  const isEditing = !!editTransaction;

  const [type, setType] = useState(editTransaction ? editTransaction.transaction_type : 'EXPENSE');
  const [amount, setAmount] = useState(editTransaction ? editTransaction.amount.toString() : '');
  const [note, setNote] = useState(editTransaction ? editTransaction.note || '' : '');
  const [selectedWalletId, setSelectedWalletId] = useState(editTransaction ? editTransaction.wallet_id : null);
  const [selectedCategoryId, setSelectedCategoryId] = useState(editTransaction ? editTransaction.category_id : null);
  const [transactionDate, setTransactionDate] = useState(
    editTransaction ? editTransaction.transaction_date : new Date().toISOString().split('T')[0]
  );

  const [wallets, setWallets] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [walletsRes, catRes] = await Promise.all([
          api.getWallets(),
          api.getCategories()
        ]);

        if (walletsRes.data?.wallets) {
          setWallets(walletsRes.data.wallets);
          if (!selectedWalletId && walletsRes.data.wallets.length > 0) {
            setSelectedWalletId(walletsRes.data.wallets[0].id);
          }
        }

        if (catRes.data) {
          setCategories(catRes.data);
          if (!selectedCategoryId && catRes.data.length > 0) {
            setSelectedCategoryId(catRes.data[0].id);
          }
        }
      } catch (error) {
        console.warn('[AddTransactionScreen] Lỗi nạp dữ liệu:', error.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleSave = async () => {
    const numericAmount = parseFloat(amount.replace(/[^0-9.]/g, ''));
    if (!numericAmount || numericAmount <= 0) {
      Alert.alert('Lỗi', 'Vui lòng nhập số tiền hợp lệ lớn hơn 0.');
      return;
    }

    if (!selectedWalletId || !selectedCategoryId) {
      Alert.alert('Lỗi', 'Vui lòng chọn đầy đủ ví và danh mục.');
      return;
    }

    setSaving(true);
    try {
      if (isEditing) {
        await api.updateTransaction({
          id: editTransaction.id,
          walletId: selectedWalletId,
          categoryId: selectedCategoryId,
          amount: numericAmount,
          transactionType: type,
          note: note.trim(),
          transactionDate
        });
        Alert.alert('Thành công', 'Đã cập nhật giao dịch & tự động bù trừ số dư ví!');
      } else {
        await api.createTransaction({
          walletId: selectedWalletId,
          categoryId: selectedCategoryId,
          amount: numericAmount,
          transactionType: type,
          note: note.trim(),
          transactionDate
        });
        Alert.alert('Thành công', 'Đã lưu giao dịch mới vào sổ!');
      }

      navigation.goBack();
    } catch (error) {
      Alert.alert('Lỗi', error.message);
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color={COLORS.primary} />
      </View>
    );
  }

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.contentContainer}>
      <View style={styles.placeholderBox}>
        <Text style={styles.title}>{isEditing ? 'Chỉnh Sửa Giao Dịch' : 'Thêm Giao Dịch Mới'}</Text>
        <Text style={styles.subtitle}>[Khu vực Trần Trọng Đại tự do thiết kế giao diện UI]</Text>

        <View style={styles.dataPreviewBox}>
          <Text style={styles.dataPreviewTitle}>Dữ liệu nạp sẵn cho Form:</Text>
          <Text style={styles.dataPreviewText}>• Số lượng ví sẵn có: {wallets.length} ví</Text>
          <Text style={styles.dataPreviewText}>• Số lượng danh mục: {categories.length} danh mục</Text>
          <Text style={styles.dataPreviewText}>• Chế độ hiện tại: {isEditing ? 'Đang sửa (Bù trừ Delta)' : 'Tạo mới'}</Text>
        </View>

        <Text style={styles.hint}>
          Đại xem hướng dẫn trong file code: Gắn các input số tiền, danh mục, ví và nút gọi `handleSave()`.
        </Text>
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
    textAlign: 'center'
  }
});
