// oidc.js
import { browser } from '$app/environment';
import { OidcClient, TokenAutomaticRenewMode, TokenRenewMode } from '@axa-fr/oidc-client';
import { isAuthLoading, tokens } from '$lib/store.js';

let ori = '.';
let href = '.';
if (browser) {
  ori = window.location.origin;
  href = window.location.href;
}

const configuration = {
  client_id: 'interactive.public.short',
  redirect_uri: ori + '/#/authentication/callback',
  silent_redirect_uri: ori + '/#/authentication/silent-callback',
  scope: 'openid profile email api',
  authority: 'https://demo.duendesoftware.com',
  service_worker_relative_url: '/OidcServiceWorker.js',
  service_worker_only: false,
  token_renew_mode: TokenRenewMode.access_token_invalid,
  token_automatic_renew_mode: TokenAutomaticRenewMode.AutomaticOnlyWhenFetchExecuted
};

const vanillaOidc = OidcClient.getOrCreate(() => fetch)(configuration);

export let getTokenAync = async () => {
  isAuthLoading.set(true);
  vanillaOidc.tryKeepExistingSessionAsync().then(() => {
    if (href.includes(configuration.redirect_uri)) {
      vanillaOidc.loginCallbackAsync().then(() => {
        window.location.href = '/';
      });

      isAuthLoading.set(true);
      return;
    }

    tokens.set(vanillaOidc.tokens);
    isAuthLoading.set(false);
  });
};

export const loginAsync = async () => {
  await vanillaOidc.loginAsync('/');
};

export const logoutAsync = async () => {
  await vanillaOidc.logoutAsync();
};
