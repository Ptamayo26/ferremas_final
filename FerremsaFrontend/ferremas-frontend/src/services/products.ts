import { apiClient } from './api';

export const getFavoritos = async () => {
  const response = await apiClient.get('/api/favoritos/mis');
  return response.data;
};
