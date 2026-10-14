import React, { createContext, useState, useEffect, useContext } from 'react';
import { Alert } from 'react-native';
import { initSocket, getSocket, disconnectSocket } from '../api/socket';
import { useAuth } from './AuthContext';

const RealtimeContext = createContext();

export const RealtimeProvider = ({ children }) => {
  const { user, logout } = useAuth();
  const [isConnected, setIsConnected] = useState(false);
  const [lastBalanceEvent, setLastBalanceEvent] = useState(null);
  const [activeNudge, setActiveNudge] = useState(null);
  const [latencyMs, setLatencyMs] = useState(null);

  useEffect(() => {
    // Nếu có user đăng nhập thì khởi tạo Socket với ID người dùng đó (hoặc mặc định User 1 nếu chưa đăng nhập để test)
    const currentUserId = user?.id || 1;
    const socket = initSocket(currentUserId, {
      deviceName: user?.full_name ? `Client của ${user.full_name}` : 'Mobile Demo Client'
    });

    const onConnect = () => {
      setIsConnected(true);
    };

    const onDisconnect = () => {
      setIsConnected(false);
    };

    // 1. Lắng nghe cập nhật số dư thời gian thực
    const onBalanceUpdate = (data) => {
      console.log('⚡ [RealtimeContext] Nhận sự kiện wallet:balance_updated:', data);
      setLastBalanceEvent(data);
    };

    // 2. Lắng nghe cảnh báo ngân sách / Nudge chi tiêu
    const onBudgetNudge = (data) => {
      console.log('📢 [RealtimeContext] Nhận cảnh báo budget:nudge_alert:', data);
      setActiveNudge(data);
    };

    // 3. Lắng nghe lệnh đăng xuất cưỡng bức (Bảo mật tài khoản)
    const onForceLogout = (data) => {
      console.warn('🔒 [RealtimeContext] Nhận lệnh auth:force_logout:', data);
      Alert.alert(
        'Đăng Xuất Bảo Mật',
        data.reason || 'Phiên làm việc của bạn đã kết thúc từ thiết bị khác.',
        [{ text: 'Đồng ý', onPress: () => logout() }]
      );
    };

    // 4. Lắng nghe phản hồi Ping kiểm tra độ trễ mạng
    const onPongReply = (data) => {
      if (data?.clientTimestamp) {
        const roundTrip = Date.now() - data.clientTimestamp;
        setLatencyMs(roundTrip);
      }
    };

    socket.on('connect', onConnect);
    socket.on('disconnect', onDisconnect);
    socket.on('wallet:balance_updated', onBalanceUpdate);
    socket.on('budget:nudge_alert', onBudgetNudge);
    socket.on('auth:force_logout', onForceLogout);
    socket.on('pong:reply', onPongReply);

    // Dọn dẹp listener khi unmount hoặc khi user thay đổi
    return () => {
      socket.off('connect', onConnect);
      socket.off('disconnect', onDisconnect);
      socket.off('wallet:balance_updated', onBalanceUpdate);
      socket.off('budget:nudge_alert', onBudgetNudge);
      socket.off('auth:force_logout', onForceLogout);
      socket.off('pong:reply', onPongReply);
    };
  }, [user]);

  // Hàm bắn Ping đo độ trễ
  const testPing = () => {
    const socket = getSocket();
    if (socket && isConnected) {
      socket.emit('ping:check', { timestamp: Date.now() });
    }
  };

  const clearNudge = () => {
    setActiveNudge(null);
  };

  return (
    <RealtimeContext.Provider
      value={{
        isConnected,
        lastBalanceEvent,
        activeNudge,
        latencyMs,
        testPing,
        clearNudge
      }}
    >
      {children}
    </RealtimeContext.Provider>
  );
};

export const useRealtime = () => {
  const context = useContext(RealtimeContext);
  if (!context) {
    throw new Error('useRealtime phải được sử dụng bên trong <RealtimeProvider>');
  }
  return context;
};

export default RealtimeContext;
