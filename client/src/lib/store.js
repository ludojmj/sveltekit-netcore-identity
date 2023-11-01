// lib/store.js
import { writable } from 'svelte/store';

export const isAuthLoading = writable(false);
export const isLoading = writable(false);
export const tokens = writable(null);
