import apiClient from "@/lib/apiClient";
import { OnBoardingDto } from "@/types/Auth/onboarding";
import {useprofile, editprofile} from "@/types/Profile/userProfile"

export const userProfile={
    getUserProfile: async(): Promise<useprofile>=>{
        return await apiClient.get<any>('/api/Profile/GetInfo');
    },
    editUserProfile:async(data: any): Promise<editprofile>=>{
        return await apiClient.put<any>('/api/Profile/Edit',data);
    },
}