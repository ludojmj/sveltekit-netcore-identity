<script>
  // Header.svelte
  import Logo from '$lib/common/Logo.svelte';
  import Logout from '$lib/oidc/Logout.svelte';
</script>

<div class="columns is-tablet">
  <div class="column">
    <a data-sveltekit-reload href="/" class="navbar-item">
      <Logo />
    </a>
  </div>

  <div class="column">
    <h1 class="title">sveltekit-netcore-identity</h1>
  </div>

  <div class="column has-text-right">
    <Logout />
  </div>
</div>
