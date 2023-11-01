// tools.js
import { goto } from '$app/navigation';
import { get } from 'svelte/store';
import { crud } from '$lib/const.js';

export const setFocus = (elt) => {
  elt.focus();
};

export const handleEscape = (event) => {
  if (event.code == 'Escape') {
    goto('/');
  }
};

export const handleFormError = (title, item, initialItem) => {
  return title !== crud.READ && title !== crud.DELETE && JSON.stringify(item) === JSON.stringify(initialItem)
    ? 'No significant changes...'
    : '';
};
