<script>
  // AuthWrapper.svelte
  import { onMount } from 'svelte';
  import { isAuthLoading, tokens } from '$lib/store.js';
  import { getTokenAync } from '$lib/oidc.js';
  import Loading from '$lib/common/Loading.svelte';
  import Login from './Login.svelte';

  onMount(async () => {
    await getTokenAync();
  });
</script>

<Login />
{#if $isAuthLoading}
  <Loading />
{:else if $tokens}
  <slot />
{:else}
  <div class="notification is-warning is-light">You need to login to access this site.</div>
{/if}
