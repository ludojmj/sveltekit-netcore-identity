// api.js
import { goto } from '$app/navigation';
import { get } from 'svelte/store';
import axios from 'axios';
import { isLoading, tokens } from '$lib/store.js';
import { apiErrMsg, crud } from '$lib/const.js';

const rootApi = import.meta.env.VITE_API_URL;
const isMock = rootApi.indexOf('mock') > -1;
const axiosCallAsync = async (params) => {
  isLoading.set(true);
  try {
    const result = await axios(params);
    return result.data;
  } catch (error) {
    return { error: getErrorMsg(error) };
  } finally {
    isLoading.set(false);
  }
};

export const apiSearchStuffAsync = async (search) => {
  const mock = isMock ? '.json' : '';
  const getMsg = {
    method: 'get',
    headers: { Authorization: `Bearer ${get(tokens).accessToken}` },
    url: `${rootApi}${mock}?search=${search}`
  };
  return axiosCallAsync(getMsg);
};

export const apiGetStuffListAsync = async () => {
  const mock = isMock ? '.json' : '';
  const getMsg = {
    method: 'get',
    headers: { Authorization: `Bearer ${get(tokens).accessToken}` },
    url: rootApi + mock
  };
  return axiosCallAsync(getMsg);
};

export const apiGotoPageAsync = async (page) => {
  const mock = isMock ? `${page}.json` : '';
  const getMsg = {
    method: 'get',
    headers: { Authorization: `Bearer ${get(tokens).accessToken}` },
    url: `${rootApi}${mock}?page=${page}`
  };
  return axiosCallAsync(getMsg);
};

export const apiGetStuffByIdAsync = async (id) => {
  const mock = isMock ? '.json' : '';
  const getMsg = {
    method: 'get',
    headers: { Authorization: `Bearer ${get(tokens).accessToken}` },
    url: `${rootApi}/${id}${mock}`
  };
  return axiosCallAsync(getMsg);
};

export const apiCreateStuffAsync = async (input) => {
  const mock = isMock ? '.json' : '';
  const postMsg = {
    method: isMock ? 'get' : 'post',
    headers: { Authorization: `Bearer ${get(tokens).accessToken}` },
    url: rootApi + mock,
    data: input
  };
  return axiosCallAsync(postMsg);
};

export const apiUpdateStuffAsync = async (id, input) => {
  const mock = isMock ? '.json' : '';
  const putMsg = {
    method: isMock ? 'get' : 'put',
    headers: { Authorization: `Bearer ${get(tokens).accessToken}` },
    url: `${rootApi}/${id}${mock}`,
    data: input
  };
  return axiosCallAsync(putMsg);
};

export const apiDeleteStuffAsync = async (id) => {
  const mock = isMock ? '.json' : '';
  const deleteMsg = {
    method: isMock ? 'get' : 'delete',
    headers: { Authorization: `Bearer ${get(tokens).accessToken}` },
    url: `${rootApi}/${id}${mock}`
  };
  return axiosCallAsync(deleteMsg);
};

const getErrorMsg = (error) => {
  const msg = apiErrMsg.generic;
  if (error.response?.status === 401) {
    return apiErrMsg.unauthorized;
  }
  if (error.response?.data?.detail) {
    return error.response.data.error;
  }
  if (error.message) {
    return error.message;
  }
  if (error) {
    return error;
  }
  return msg;
};

export const crudApiCallAsync = async (crudTitle, item) => {
  delete item.error;
  let result;

  if (crudTitle === crud.READ) {
    goto('/');
    return item;
  }

  if (crudTitle === crud.CREATE) {
    result = await apiCreateStuffAsync(item);
  }

  if (crudTitle === crud.UPDATE) {
    result = await apiUpdateStuffAsync(item.id, item);
  }

  if (crudTitle === crud.DELETE) {
    result = await apiDeleteStuffAsync(item.id);
  }

  if (result.error) {
    item.error = result.error;
    return item;
  }

  goto('/');
  return item;
};

export const getItemAsync = async (crudTitle, id) => {
  let item = {};
  const initialItem = {};

  if (crudTitle !== crud.CREATE) {
    item = await apiGetStuffByIdAsync(id);
  }

  for (let key in item) {
    initialItem[key] = item[key];
  }

  return { item, initialItem };
};
