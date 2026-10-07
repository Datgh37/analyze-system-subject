import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useRealtime } from '../context/RealtimeContext';
import { COLORS } from '../constants/theme';

/**
 * ⚡ BANNER THÔNG BÁO THỜI GIAN THỰC (SOCKET.IO REALTIME BANNER)
 * Hiển thị tức thì các sự kiện bắn từ Server: Cảnh báo ngân sách (Nudge), biến động số dư ví.
 */
export default function RealtimeAlertBanner() {
  const { isConnected, activeNudge, lastBalanceEvent, latencyMs, clearNudge } = useRealtime();

  return (
    <View style={styles.wrapper}>
      {/* 1. Chỉ báo trạng thái kết nối Socket.IO */}
      <View style={styles.statusBar}>
        <View style={[styles.statusDot, isConnected ? styles.dotConnected : styles.dotDisconnected]} />
        <Text style={styles.statusText}>
          {isConnected ? `Realtime Socket.IO: Sẵn sàng${latencyMs ? ` (${latencyMs}ms)` : ''}` : 'Realtime: Đang kết nối...'}
        </Text>
      </View>

      {/* 2. Cảnh báo Ngân sách (Nudge Alert) nếu có */}
      {activeNudge && (
        <View style={[styles.alertCard, activeNudge.severity === 'danger' ? styles.cardDanger : styles.cardWarning]}>
          <View style={styles.alertHeader}>
            <Ionicons 
              name={activeNudge.severity === 'danger' ? 'alert-circle' : 'warning-outline'} 
              size={20} 
              color={activeNudge.severity === 'danger' ? '#D32F2F' : '#E65100'} 
            />
            <Text style={[styles.alertTitle, activeNudge.severity === 'danger' ? styles.textDanger : styles.textWarning]}>
              {activeNudge.alertTitle}
            </Text>
            <TouchableOpacity onPress={clearNudge} style={styles.closeBtn}>
              <Ionicons name="close" size={18} color="#757575" />
            </TouchableOpacity>
          </View>
          <Text style={styles.alertMessage}>{activeNudge.alertMessage}</Text>
        </View>
      )}

      {/* 3. Thông báo Biến động Số dư Ví tức thời (Balance Sync Event) */}
      {lastBalanceEvent && !activeNudge && (
        <View style={styles.balanceCard}>
          <Ionicons name="flash" size={18} color="#2E7D32" />
          <View style={styles.balanceInfo}>
            <Text style={styles.balanceTitle}>
              Ví {lastBalanceEvent.walletName}: {lastBalanceEvent.delta > 0 ? '+' : ''}
              {Number(lastBalanceEvent.delta).toLocaleString('vi-VN')} đ
            </Text>
            <Text style={styles.balanceSub}>
              Số dư mới: {Number(lastBalanceEvent.newBalance).toLocaleString('vi-VN')} đ ({lastBalanceEvent.reason})
            </Text>
          </View>
        </View>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  wrapper: {
    paddingHorizontal: 16,
    paddingTop: 8,
    paddingBottom: 4,
    backgroundColor: '#FFFFFF',
    borderBottomWidth: 1,
    borderBottomColor: '#EEEEEE'
  },
  statusBar: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 6
  },
  statusDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    marginRight: 6
  },
  dotConnected: {
    backgroundColor: '#2E7D32' // Xanh lá
  },
  dotDisconnected: {
    backgroundColor: '#B0BEC5' // Xám
  },
  statusText: {
    fontSize: 11,
    color: '#757575',
    fontWeight: '500'
  },
  alertCard: {
    borderRadius: 8,
    padding: 10,
    marginBottom: 4,
    borderWidth: 1
  },
  cardWarning: {
    backgroundColor: '#FFF8E1',
    borderColor: '#FFE082'
  },
  cardDanger: {
    backgroundColor: '#FFEBEE',
    borderColor: '#FFCDD2'
  },
  alertHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 4
  },
  alertTitle: {
    fontSize: 13,
    fontWeight: '700',
    marginLeft: 6,
    flex: 1
  },
  textWarning: {
    color: '#E65100'
  },
  textDanger: {
    color: '#D32F2F'
  },
  alertMessage: {
    fontSize: 12,
    color: '#424242',
    lineHeight: 16
  },
  closeBtn: {
    padding: 2
  },
  balanceCard: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#E8F5E9',
    borderRadius: 8,
    padding: 8,
    marginBottom: 4,
    borderWidth: 1,
    borderColor: '#C8E6C9'
  },
  balanceInfo: {
    marginLeft: 8,
    flex: 1
  },
  balanceTitle: {
    fontSize: 12,
    fontWeight: '700',
    color: '#1B5E20'
  },
  balanceSub: {
    fontSize: 11,
    color: '#388E3C'
  }
});
