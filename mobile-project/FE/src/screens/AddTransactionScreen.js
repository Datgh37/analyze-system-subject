import React, { useState } from 'react';
import { View, Text, StyleSheet, TextInput, TouchableOpacity, ScrollView, Alert } from 'react-native';
import { COLORS, SIZES } from '../constants/theme';
import apiClient from '../api/client';

/**
 * MÀN HÌNH THÊM GIAO DỊCH (ADD TRANSACTION)
 * Kiến trúc & Kết nối API: Trịnh Thành Đạt (Lead)
 * Phụ trách hoàn thiện UI & Form: Trần Trọng Đại (UI Specialist)
 * 
 * 👉 Nhiệm vụ Đại add nốt:
 * - Bổ sung Lưới chọn danh mục (Grid View) có icon và màu sắc
 * - Tích hợp DateTimePicker để chọn ngày thực tế
 */
export default function AddTransactionScreen({ navigation }) {
  const [type, setType] = useState('EXPENSE');
  const [amount, setAmount] = useState('');
  const [note, setNote] = useState('');

  const handleSave = async () => {
    if (!amount || parseFloat(amount) <= 0) {
      Alert.alert('Thông báo', 'Vui lòng nhập số tiền hợp lệ (> 0 đ).');
      return;
    }

    try {
      const payload = {
        user_id: 1,
        wallet_id: 1,
        category_id: type === 'EXPENSE' ? 1 : 9,
        amount: parseFloat(amount),
        transaction_type: type,
        note: note || (type === 'EXPENSE' ? 'Chi tiêu cá nhân' : 'Khoản thu nhập'),
        transaction_date: new Date().toISOString().split('T')[0]
      };

      const res = await apiClient.post('/transactions/create.php', payload);
      if (res && res.status === 'success') {
        Alert.alert('Thành công', 'Đã lưu giao dịch mới!', [
          { text: 'OK', onPress: () => navigation.navigate('Trang Chủ') }
        ]);
        setAmount('');
        setNote('');
      }
    } catch (err) {
      Alert.alert('Lỗi', err.message);
    }
  };

  return (
    <ScrollView style={styles.container}>
      {/* 1. Nút chuyển đổi loại Thu / Chi */}
      <View style={styles.tabSwitch}>
        <TouchableOpacity
          style={[styles.switchButton, type === 'EXPENSE' && styles.switchExpenseActive]}
          onPress={() => setType('EXPENSE')}
        >
          <Text style={[styles.switchText, type === 'EXPENSE' && styles.switchTextActive]}>
            Khoản Chi (-)
          </Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[styles.switchButton, type === 'INCOME' && styles.switchIncomeActive]}
          onPress={() => setType('INCOME')}
        >
          <Text style={[styles.switchText, type === 'INCOME' && styles.switchTextActive]}>
            Khoản Thu (+)
          </Text>
        </TouchableOpacity>
      </View>

      {/* 2. Ô nhập số tiền */}
      <View style={styles.inputCard}>
        <Text style={styles.inputLabel}>Số Tiền (VND)</Text>
        <TextInput
          style={styles.amountInput}
          keyboardType="numeric"
          placeholder="0"
          placeholderTextColor={COLORS.textSecondary}
          value={amount}
          onChangeText={setAmount}
        />
      </View>

      {/* 3. Ô nhập ghi chú */}
      <View style={styles.inputCard}>
        <Text style={styles.inputLabel}>Ghi Chú</Text>
        <TextInput
          style={styles.textInput}
          placeholder="Ví dụ: Cà phê sáng, Mua sách..."
          placeholderTextColor={COLORS.textSecondary}
          value={note}
          onChangeText={setNote}
        />
      </View>

      {/* 4. Nút bấm Lưu */}
      <TouchableOpacity style={styles.saveButton} onPress={handleSave}>
        <Text style={styles.saveButtonText}>Lưu Giao Dịch</Text>
      </TouchableOpacity>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background,
    padding: SIZES.md
  },
  tabSwitch: {
    flexDirection: 'row',
    backgroundColor: COLORS.border,
    borderRadius: SIZES.radiusMd,
    padding: 4,
    marginBottom: SIZES.md
  },
  switchButton: {
    flex: 1,
    paddingVertical: SIZES.sm,
    borderRadius: SIZES.radiusSm,
    alignItems: 'center'
  },
  switchExpenseActive: {
    backgroundColor: COLORS.expense
  },
  switchIncomeActive: {
    backgroundColor: COLORS.income
  },
  switchText: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.textSecondary
  },
  switchTextActive: {
    color: COLORS.white
  },
  inputCard: {
    backgroundColor: COLORS.card,
    borderRadius: SIZES.radiusMd,
    padding: SIZES.md,
    marginBottom: SIZES.md,
    elevation: 2
  },
  inputLabel: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginBottom: SIZES.xs
  },
  amountInput: {
    fontSize: SIZES.fontTitle,
    fontWeight: 'bold',
    color: COLORS.text,
    borderBottomWidth: 1,
    borderBottomColor: COLORS.border,
    paddingVertical: SIZES.xs
  },
  textInput: {
    fontSize: SIZES.fontMd,
    color: COLORS.text,
    paddingVertical: SIZES.xs
  },
  saveButton: {
    backgroundColor: COLORS.primary,
    paddingVertical: SIZES.md,
    borderRadius: SIZES.radiusMd,
    alignItems: 'center',
    marginTop: SIZES.sm
  },
  saveButtonText: {
    color: COLORS.white,
    fontSize: SIZES.fontLg,
    fontWeight: 'bold'
  }
});
