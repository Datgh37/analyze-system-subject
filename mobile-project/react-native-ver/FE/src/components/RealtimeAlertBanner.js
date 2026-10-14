import React, { useState } from 'react';
import { View, Text, TouchableOpacity, ActivityIndicator } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useRealtime } from '../context/RealtimeContext';
import { realtimeBannerStyles as styles } from '../constants/appStyles';

/**
 * ⚡ BANNER THÔNG BÁO THỜI GIAN THỰC (SOCKET.IO REALTIME BANNER)
 * - Hiển thị trạng thái kết nối Socket.IO thời gian thực & độ trễ mạng (Ping ms).
 * - Cung cấp nút [⚡ Test Bắn Sự Kiện] trực tiếp để giáo viên và người dùng kiểm chứng Socket.IO tức thì!
 * - Tự động hiển thị thẻ thông báo nổi khi có biến động số dư hoặc cảnh báo ngân sách.
 * - Styling tập trung quản lý tại FE/src/constants/appStyles.js
 */
export default function RealtimeAlertBanner() {
  const { 
    isConnected, 
    activeNudge, 
    lastBalanceEvent, 
    latencyMs, 
    clearNudge, 
    clearBalanceEvent, 
    triggerDemoEvent 
  } = useRealtime();

  const [testing, setTesting] = useState(false);

  const handleTestTrigger = async () => {
    if (testing) return;
    setTesting(true);
    try {
      if (typeof triggerDemoEvent === 'function') {
        await triggerDemoEvent();
      }
    } finally {
      setTimeout(() => setTesting(false), 800);
    }
  };

  return (
    <View style={styles.wrapper}>
      {/* 1. Thanh trạng thái & Nút Test Socket.IO trực tiếp */}
      <View style={styles.statusBar}>
        <View style={styles.statusLeft}>
          <View style={[styles.statusDot, isConnected ? styles.dotConnected : styles.dotDisconnected]} />
          <Text style={styles.statusText}>
            {isConnected ? 'Socket.IO: Sẵn sàng' : 'Socket.IO: Đang kết nối...'}
          </Text>
          {latencyMs ? (
            <View style={styles.latencyBadge}>
              <Text style={styles.latencyText}>{latencyMs}ms</Text>
            </View>
          ) : null}
        </View>

        {isConnected && (
          <TouchableOpacity 
            style={styles.testBtn} 
            onPress={handleTestTrigger}
            activeOpacity={0.7}
            disabled={testing}
          >
            {testing ? (
              <ActivityIndicator size="small" color="#FFFFFF" style={{ marginRight: 4 }} />
            ) : (
              <Ionicons name="flash" size={13} color="#FBBF24" style={{ marginRight: 3 }} />
            )}
            <Text style={styles.testBtnText}>
              {testing ? 'Đang bắn...' : 'Test Realtime'}
            </Text>
          </TouchableOpacity>
        )}
      </View>

      {/* 2. Cảnh báo Ngân sách (Nudge Alert) từ Server */}
      {activeNudge && (
        <View style={[styles.alertCard, activeNudge.severity === 'danger' ? styles.cardDanger : styles.cardWarning]}>
          <View style={styles.alertHeader}>
            <Ionicons 
              name={activeNudge.severity === 'danger' ? 'alert-circle' : 'warning-outline'} 
              size={18} 
              color={activeNudge.severity === 'danger' ? '#DC2626' : '#D97706'} 
            />
            <Text style={[styles.alertTitle, activeNudge.severity === 'danger' ? styles.textDanger : styles.textWarning]}>
              {activeNudge.alertTitle || 'CẢNH BÁO CHI TIÊU'}
            </Text>
            <TouchableOpacity onPress={clearNudge} style={styles.closeBtn} hitSlop={{ top: 8, bottom: 8, left: 8, right: 8 }}>
              <Ionicons name="close" size={16} color="#64748B" />
            </TouchableOpacity>
          </View>
          <Text style={styles.alertMessage}>{activeNudge.alertMessage}</Text>
        </View>
      )}

      {/* 3. Thông báo Biến động Số dư Ví tức thời (Realtime Balance Sync Event) */}
      {lastBalanceEvent && !activeNudge && (
        <View style={styles.balanceCard}>
          <View style={styles.balanceIconWrap}>
            <Ionicons name="flash" size={18} color="#059669" />
          </View>
          <View style={styles.balanceInfo}>
            <Text style={styles.balanceTitle}>
              Đồng bộ số dư: <Text style={{ color: Number(lastBalanceEvent.delta) >= 0 ? '#059669' : '#DC2626', fontWeight: 'bold' }}>
                {Number(lastBalanceEvent.delta) >= 0 ? '+' : ''}
                {Number(lastBalanceEvent.delta).toLocaleString('vi-VN')} đ
              </Text>
            </Text>
            <Text style={styles.balanceSub} numberOfLines={1}>
              {lastBalanceEvent.walletName} • Số dư mới: {Number(lastBalanceEvent.newBalance).toLocaleString('vi-VN')} đ
            </Text>
          </View>
          <TouchableOpacity onPress={clearBalanceEvent} style={styles.closeBtn} hitSlop={{ top: 8, bottom: 8, left: 8, right: 8 }}>
            <Ionicons name="close" size={16} color="#64748B" />
          </TouchableOpacity>
        </View>
      )}
    </View>
  );
}

