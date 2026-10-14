import React from 'react';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { Ionicons } from '@expo/vector-icons';
import HomeScreen from '../screens/main/HomeScreen';
import TransactionsScreen from '../screens/main/TransactionsScreen';
import BudgetScreen from '../screens/main/BudgetScreen';
import ReportScreen from '../screens/main/ReportScreen';
import ProfileScreen from '../screens/main/ProfileScreen';
import { COLORS } from '../constants/appStyles';

const Tab = createBottomTabNavigator();

export default function TabNavigator() {
  return (
    <Tab.Navigator
      screenOptions={({ route }) => ({
        headerShown: true,
        headerStyle: {
          backgroundColor: COLORS.surface,
          elevation: 1,
          shadowOpacity: 0.05
        },
        headerTitleStyle: {
          fontWeight: 'bold',
          color: COLORS.primaryDark
        },
        tabBarActiveTintColor: COLORS.primary,
        tabBarInactiveTintColor: COLORS.textMuted,
        tabBarStyle: {
          backgroundColor: COLORS.surface,
          borderTopColor: COLORS.border,
          height: 60,
          paddingBottom: 8,
          paddingTop: 8
        },
        tabBarLabelStyle: {
          fontSize: 11,
          fontWeight: '600'
        },
        tabBarIcon: ({ focused, color, size }) => {
          let iconName;

          if (route.name === 'Home') {
            iconName = focused ? 'home' : 'home-outline';
          } else if (route.name === 'Transactions') {
            iconName = focused ? 'receipt' : 'receipt-outline';
          } else if (route.name === 'Budget') {
            iconName = focused ? 'pie-chart' : 'pie-chart-outline';
          } else if (route.name === 'Reports') {
            iconName = focused ? 'bar-chart' : 'bar-chart-outline';
          } else if (route.name === 'Profile') {
            iconName = focused ? 'person' : 'person-outline';
          }

          return <Ionicons name={iconName} size={22} color={color} />;
        }
      })}
    >
      <Tab.Screen
        name="Home"
        component={HomeScreen}
        options={{ title: 'Tổng Quan', headerTitle: 'Smart Expense' }}
      />
      <Tab.Screen
        name="Transactions"
        component={TransactionsScreen}
        options={{ title: 'Sổ Giao Dịch', headerTitle: 'Lịch Sử Thu - Chi' }}
      />
      <Tab.Screen
        name="Budget"
        component={BudgetScreen}
        options={{ title: 'Ngân Sách', headerTitle: 'Hạn Mức Ngân Sách' }}
      />
      <Tab.Screen
        name="Reports"
        component={ReportScreen}
        options={{ title: 'Báo Cáo', headerTitle: 'Báo Cáo & Thống Kê' }}
      />
      <Tab.Screen
        name="Profile"
        component={ProfileScreen}
        options={{ title: 'Hồ Sơ', headerTitle: 'Tài Khoản Cá Nhân' }}
      />
    </Tab.Navigator>
  );
}
