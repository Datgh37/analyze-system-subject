import React from 'react';
import { StatusBar } from 'expo-status-bar';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { NavigationContainer } from '@react-navigation/native';
import AppNavigator from './src/navigation/AppNavigator';

/**
 * ĐIỂM KHỞI CHẠY ỨNG DỤNG (ENTRY POINT)
 * Đồ án môn học: Lập trình Thiết bị Di động
 * Sinh viên thực hiện:
 * 1. Trịnh Thành Đạt (Lead - Kiến trúc Backend & Setup FE)
 * 2. Trần Trọng Đại (UI Specialist & Frontend Screens)
 */
export default function App() {
  return (
    <SafeAreaProvider>
      <NavigationContainer>
        <StatusBar style="auto" />
        <AppNavigator />
      </NavigationContainer>
    </SafeAreaProvider>
  );
}
