import React from 'react';
import { StatusBar } from 'expo-status-bar';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { AuthProvider } from './src/context/AuthContext';
import { RealtimeProvider } from './src/context/RealtimeContext';
import AppNavigator from './src/navigation/AppNavigator';

/**
 * ĐỒ ÁN MÔN HỌC: LẬP TRÌNH THIẾT BỊ DI ĐỘNG (DNTU)
 * ĐỀ TÀI: ỨNG DỤNG QUẢN LÝ CHI TIÊU CÁ NHÂN THÔNG MINH
 * 
 * Nhóm thực hiện:
 * - Trịnh Thành Đạt (Lead & Backend Architecture)
 * - Trần Trọng Đại (UI & Mobile Frontend Lead)
 * 
 * Ngăn xếp công nghệ:
 * - Expo SDK 54 (~54.0.36)
 * - React Native 0.81.5 & React 19.1.0
 * - React Navigation 7.x
 * - Socket.IO Client 4.7.5 (Realtime Full-Duplex Events)
 * - Pure StyleSheet & COLORS tokens
 */

export default function App() {
  return (
    <SafeAreaProvider>
      <AuthProvider>
        <RealtimeProvider>
          <StatusBar style="dark" />
          <AppNavigator />
        </RealtimeProvider>
      </AuthProvider>
    </SafeAreaProvider>
  );
}
