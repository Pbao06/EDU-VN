import {useState,useCallback} from 'react';
import { userProfile } from '@/services/profileService';
import{useprofile,editprofile} from '@/types/Profile/userProfile';

export const useProfile=()=>{
    const [loading,SetLoading]=useState(false);

    const getInfoUser = useCallback(async ()=>{
        SetLoading(true);
        try
        {
            return await userProfile.getUserProfile();
        }finally{
            SetLoading(false);
        }
    },[]);
    
    const editInfoUser= useCallback(async (data:editprofile)=>{
        SetLoading(true);
        try{
            return await userProfile.editUserProfile(data)
        }finally{
            SetLoading(false);
        }
    },[]);

    return {
        getInfoUser, editInfoUser,loading
    };
}