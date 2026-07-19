"use client";  
import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { authService } from "@/services/authService";
import { LoginDto, RegisterDto } from "@/types/Auth/register";

export interface UserInfo {
    email: string;
    fullName: string;
    userId: string;
}

export const useAuth = () => {
    const router = useRouter();
    const [user, setUser] = useState<UserInfo | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Chỉ set thông tin user từ localStorage sau khi component đã Mount trên browser (tránh lỗi SSR)
    useEffect(() => {
        const storedUser = localStorage.getItem("user");
        if (storedUser) {
            try {
                setUser(JSON.parse(storedUser));
            } catch (e) {
                localStorage.removeItem("user");
            }
        }
    }, []);

    const login = async (data: LoginDto) => {
        setLoading(true); 
        setError(null);
        try {
            const response = await authService.login(data);
            localStorage.setItem("token", response.accessToken);
            localStorage.setItem("refreshToken", response.refreshToken);
            
            const userInfo: UserInfo = {
                email: response.email,
                fullName: response.fullName,
                userId: response.userId
            };
            localStorage.setItem("user", JSON.stringify(userInfo));
            setUser(userInfo);
            router.push("/home"); // Next.js route nên để chữ thường
            return response;
        } catch (err: any) {
            setError(err.message || "Đăng nhập thất bại");
            throw err;
        } finally {
            setLoading(false);
        }
    };

    const register = async (data: RegisterDto) => {
        setLoading(true);
        setError(null);
        try {
            const response = await authService.register(data);
            localStorage.setItem("token", response.accessToken);
            localStorage.setItem("refreshToken", response.refreshToken);
            
            const userInfo: UserInfo = {
                email: response.email,
                fullName: response.fullName,
                userId: response.userId
            };
            localStorage.setItem("user", JSON.stringify(userInfo));
            setUser(userInfo);
            router.push("/home");
            return response;
        } catch (err: any) {
            setError(err.message || "Đăng ký thất bại");
            throw err;
        } finally {
            setLoading(false);
        }
    };

    const logout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("user");
        setUser(null);
        router.push("/login");
    };

    return {
        user,
        isAuthenticated: !!user,
        loading,
        error,
        login,
        register,
        logout,
    };
};