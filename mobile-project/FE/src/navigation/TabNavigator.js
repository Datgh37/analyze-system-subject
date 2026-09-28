import React from 'react';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { Ionicons } from '@expo/vector-icons';
import { COLORS } from '../constants/theme';

import HomeScreen from '../screens/HomeScreen';
import AddTransactionScreen from '../screens/AddTransactionScreen';
import HistoryScreen from '../screens/HistoryScreen';
import ReportScreen from '../screens/ReportScreen';

const Tab = createBottomTabNavigator();

/**
 * ĐIỀU HƯỚNG TAB CHÍNH (BOTTOM TAB NAVIGATOR)
 * Thiết kế kiến trúc: Trịnh Thành Đạt (Lead)
 */
export default function TabNavigator() {
  return (
    <Tab.Navigator
      screenOptions={({ route }) => ({
        headerShown: false,
        tabBarActiveTintColor: COLORS.primary,
        tabBarInactiveTintColor: COLORS.textSecondary,
        tabBarStyle: {
          backgroundColor: COLORS.card,
          height: 60,
          paddingBottom: 8,
          paddingTop: 6
        },
        tabBarIcon: ({ focused, color, size }) => {
          let iconName;

          if (route.name === 'Trang Chủ') {
            iconName = focused ? 'home' : 'home-outline';
          } else if (route.name === 'Thêm Mới') {
            iconName = focused ? 'add-circle' : 'add-circle-outline';
          } else if (route.name === 'Lịch Sử') {
            iconName = focused ? 'time' : 'time-outline';
          } else if (route.name === 'Báo Cáo') {
            iconName = focused ? 'pie-chart' : 'pie-chart-outline';
          }

          return <Ionicons name={iconName} size={size} color={color} />;
        }
      })}
    >
      <Tab.Screen name="Trang Chủ" component={HomeScreen} />
      <Tab.Screen name="Thêm Mới" component={AddTransactionScreen} />
      <Tab.Screen name="Lịch Sử" component={HistoryScreen} />
      <Tab.Screen name="Báo Cáo" component={ReportScreen} />
    </Tab.Navigator>
  );
}
