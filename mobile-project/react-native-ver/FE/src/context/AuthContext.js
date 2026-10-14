import React, { createContext, useState, useEffect, useContext } from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { api } from '../api/endpoints';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  // 1. Kiểm tra phiên đăng nhập đã lưu trong AsyncStorage khi mở ứng dụng
  useEffect(() => {
    const loadSession = async () => {
      try {
        const storedToken = await AsyncStorage.getItem('@auth_token');
        const storedUser  = await AsyncStorage.getItem('@user_profile');

        if (storedToken && storedUser) {
          setToken(storedToken);
          setUser(JSON.parse(storedUser));
        }
      } catch (error) {
        console.warn('[AuthContext] Lỗi khi nạp phiên làm việc:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadSession();
  }, []);

  // 2. Hàm Đăng nhập
  const login = async (email, password) => {
    setIsLoading(true);
    try {
      const response = await api.login(email, password);
      const { user: userData, token: jwtToken } = response.data;

      setToken(jwtToken);
      setUser(userData);

      await AsyncStorage.setItem('@auth_token', jwtToken);
      await AsyncStorage.setItem('@user_profile', JSON.stringify(userData));

      return { success: true };
    } catch (error) {
      return { success: false, message: error.message };
    } finally {
      setIsLoading(false);
    }
  };

  // 3. Hàm Đăng ký
  const register = async (fullName, email, password) => {
    setIsLoading(true);
    try {
      const response = await api.register(fullName, email, password);
      const { user: userData, token: jwtToken } = response.data;

      setToken(jwtToken);
      setUser(userData);

      await AsyncStorage.setItem('@auth_token', jwtToken);
      await AsyncStorage.setItem('@user_profile', JSON.stringify(userData));

      return { success: true };
    } catch (error) {
      return { success: false, message: error.message };
    } finally {
      setIsLoading(false);
    }
  };

  // 4. Hàm Đăng xuất
  const logout = async () => {
    setIsLoading(true);
    try {
      await AsyncStorage.multiRemove(['@auth_token', '@user_profile']);
      setToken(null);
      setUser(null);
    } catch (error) {
      console.warn('[AuthContext] Lỗi đăng xuất:', error);
    } finally {
      setIsLoading(false);
    }
  };

  // 5. Hàm cập nhật thông tin user trong state & AsyncStorage sau khi đổi tên/mật khẩu
  const updateUser = async (updatedData) => {
    const newUser = { ...user, ...updatedData };
    setUser(newUser);
    await AsyncStorage.setItem('@user_profile', JSON.stringify(newUser));
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token,
        isLoading,
        login,
        register,
        logout,
        updateUser
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth phải được dùng bên trong <AuthProvider>');
  }
  return context;
};

export default AuthContext;
