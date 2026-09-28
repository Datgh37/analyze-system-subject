import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, FlatList, TouchableOpacity, Alert, ActivityIndicator } from 'react-native';
import { COLORS, SIZES } from '../constants/theme';
import apiClient from '../api/client';

/**
 * MÀN HÌNH LỊCH SỬ THU CHI (HISTORY SCREEN)
 * Kiến trúc & Kết nối API: Trịnh Thành Đạt (Lead)
 * Phụ trách hoàn thiện UI & FlatList: Trần Trọng Đại (UI Specialist)
 * 
 * 👉 Nhiệm vụ Đại add nốt:
 * - Tách dòng giao dịch thành: src/components/TransactionItem.js
 * - Thêm bộ lọc chọn Tháng / Năm để lọc dữ liệu
 */
export default function HistoryScreen() {
  const [loading, setLoading] = useState(true);
  const [transactions, setTransactions] = useState([]);

  useEffect(() => {
    fetchTransactions();
  }, []);

  const fetchTransactions = async () => {
    try {
      setLoading(true);
      const res = await apiClient.get('/transactions/list.php?user_id=1&limit=30');
      if (res && res.data && res.data.transactions) {
        setTransactions(res.data.transactions);
      }
    } catch (err) {
      console.log('Lỗi tải lịch sử:', err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = (item) => {
    Alert.alert(
      'Xác nhận xóa',
      `Bạn có chắc muốn xóa giao dịch "${item.note || item.category_name}" (${item.amount.toLocaleString('vi-VN')} đ) không?`,
      [
        { text: 'Hủy', style: 'cancel' },
        {
          text: 'Xóa',
          style: 'destructive',
          onPress: async () => {
            try {
              await apiClient.post('/transactions/delete.php', {
                user_id: 1,
                transaction_id: item.id
              });
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

  const renderItem = ({ item }) => {
    const isExpense = item.transaction_type === 'EXPENSE';
    return (
      <View style={styles.itemRow}>
        <View style={styles.itemLeft}>
          <Text style={styles.itemTitle}>{item.category_name || 'Khác'}</Text>
          <Text style={styles.itemSubtitle}>{item.note || 'Không có ghi chú'}</Text>
          <Text style={styles.itemDate}>{item.transaction_date}</Text>
        </View>
        <View style={styles.itemRight}>
          <Text style={[styles.itemAmount, isExpense ? styles.expenseColor : styles.incomeColor]}>
            {isExpense ? '-' : '+'}
            {item.amount.toLocaleString('vi-VN')} đ
          </Text>
          <TouchableOpacity onPress={() => handleDelete(item)}>
            <Text style={styles.deleteButton}>Xóa</Text>
          </TouchableOpacity>
        </View>
      </View>
    );
  };

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.headerTitle}>Lịch Sử Giao Dịch Tháng 09/2026</Text>
      </View>

      {loading ? (
        <ActivityIndicator size="large" color={COLORS.primary} style={{ marginTop: 40 }} />
      ) : (
        <FlatList
          data={transactions}
          keyExtractor={(item) => item.id.toString()}
          renderItem={renderItem}
          contentContainerStyle={styles.listContent}
          ListEmptyComponent={
            <Text style={styles.emptyText}>Chưa có giao dịch nào được ghi nhận.</Text>
          }
        />
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background
  },
  header: {
    padding: SIZES.md,
    backgroundColor: COLORS.card,
    borderBottomWidth: 1,
    borderBottomColor: COLORS.border
  },
  headerTitle: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.text
  },
  listContent: {
    padding: SIZES.md
  },
  itemRow: {
    backgroundColor: COLORS.card,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: SIZES.md,
    borderRadius: SIZES.radiusMd,
    marginBottom: SIZES.sm,
    elevation: 1
  },
  itemLeft: {
    flex: 1
  },
  itemTitle: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.text
  },
  itemSubtitle: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginTop: 2
  },
  itemDate: {
    fontSize: SIZES.fontSm - 2,
    color: COLORS.textSecondary,
    marginTop: 4
  },
  itemRight: {
    alignItems: 'flex-end'
  },
  itemAmount: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold'
  },
  expenseColor: {
    color: COLORS.expense
  },
  incomeColor: {
    color: COLORS.income
  },
  deleteButton: {
    color: COLORS.danger,
    fontSize: SIZES.fontSm,
    marginTop: 6,
    textDecorationLine: 'underline'
  },
  emptyText: {
    textAlign: 'center',
    marginTop: 40,
    color: COLORS.textSecondary,
    fontSize: SIZES.fontMd
  }
});
