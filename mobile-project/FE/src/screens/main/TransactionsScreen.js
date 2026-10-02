import React, { useState, useEffect, useCallback } from 'react';
import { View, Text, StyleSheet, ActivityIndicator, RefreshControl, ScrollView, Alert } from 'react-native';
import { api } from '../../api/endpoints';
import { COLORS, SPACING, SIZES } from '../../constants/theme';

/**
 * ==============================================================================
 * MÀN HÌNH: SỔ GIAO DỊCH (TRANSACTIONS SCREEN)
 * Phụ trách Giao diện: Trần Trọng Đại (UI Lead)
 * Logic Backend & API: Đã tích hợp sẵn 100% bên dưới
 * ==============================================================================
 * 
 * 📌 CÁC BIẾN & HÀM LOGIC CÓ SẴN ĐỂ ĐẠI DÙNG:
 * 1. State danh sách & Bộ lọc đa tiêu chí:
 *    - `transactions`: Mảng toàn bộ giao dịch lấy từ API
 *    - `selectedType`, `setSelectedType`: Lọc theo loại ('EXPENSE' | 'INCOME' | null = Tất cả)
 *    - `keyword`, `setKeyword`: Từ khóa tìm kiếm trong ghi chú
 *    - `currentMonth`, `setCurrentMonth`: Lọc theo tháng
 *    - `currentYear`, `setCurrentYear`: Lọc theo năm
 * 
 * 2. Hàm xử lý (Handlers):
 *    - `fetchTransactions()`: Gọi API lấy danh sách theo bộ lọc
 *    - `handleDeleteTransaction(id, note)`: Gọi API xóa giao dịch (Backend tự hoàn tiền vào ví)
 *    - `onRefresh()`: Kéo xuống làm mới
 *    - `formatCurrency(amount)`: Định dạng VND
 * 
 * 3. Chuyển sang sửa giao dịch:
 *    - `navigation.navigate('AddTransaction', { transaction: item })`: Mở màn hình sửa
 * ==============================================================================
 */

export default function TransactionsScreen({ navigation }) {
  const [transactions, setTransactions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [selectedType, setSelectedType] = useState(null);
  const [keyword, setKeyword] = useState('');
  const [currentMonth, setCurrentMonth] = useState(new Date().getMonth() + 1);
  const [currentYear, setCurrentYear] = useState(new Date().getFullYear());

  const fetchTransactions = async () => {
    try {
      const res = await api.getTransactions({
        month: currentMonth,
        year: currentYear,
        type: selectedType,
        keyword: keyword.trim()
      });
      if (res.data) {
        setTransactions(res.data.transactions || []);
      }
    } catch (error) {
      console.warn('[TransactionsScreen] Lỗi tải giao dịch:', error.message);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    fetchTransactions();
  }, [selectedType, currentMonth, currentYear]);

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    fetchTransactions();
  }, [selectedType, keyword, currentMonth, currentYear]);

  const handleDeleteTransaction = (id, note) => {
    Alert.alert(
      'Xác nhận xóa',
      `Bạn có chắc chắn muốn xóa giao dịch "${note || 'này'}"? Số dư ví sẽ được tự động hoàn trả.`,
      [
        { text: 'Hủy', style: 'cancel' },
        {
          text: 'Xóa ngay',
          style: 'destructive',
          onPress: async () => {
            try {
              await api.deleteTransaction(id);
              Alert.alert('Thành công', 'Đã xóa giao dịch và hoàn trả số dư ví.');
              fetchTransactions();
            } catch (err) {
              Alert.alert('Lỗi', err.message);
            }
          }
        }
      ]
    );
  };

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount || 0);
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color={COLORS.primary} />
      </View>
    );
  }

  return (
    <ScrollView
      style={styles.container}
      contentContainerStyle={styles.contentContainer}
      refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={[COLORS.primary]} />}
    >
      <View style={styles.placeholderBox}>
        <Text style={styles.title}>Màn hình Sổ Giao Dịch</Text>
        <Text style={styles.subtitle}>[Khu vực Trần Trọng Đại tự do thiết kế giao diện UI]</Text>

        <View style={styles.dataPreviewBox}>
          <Text style={styles.dataPreviewTitle}>Trạng thái kết nối API Backend:</Text>
          <Text style={styles.dataPreviewText}>• Đã nạp thành công: {transactions.length} giao dịch từ MySQL</Text>
          <Text style={styles.dataPreviewText}>• Bộ lọc hiện tại: Loại = {selectedType || 'Tất cả'}, Tháng = {currentMonth}/{currentYear}</Text>
        </View>

        <Text style={styles.hint}>
          Đại xem hướng dẫn trong file code: Gắn FlatList render mảng `transactions`, gọi `handleDeleteTransaction(id)` khi xóa.
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
