import{w as u}from"./index.ee75ec63.js";var p;const k=((p=globalThis.__sveltekit_1oqobcm)==null?void 0:p.base)??"";var h;const m=((h=globalThis.__sveltekit_1oqobcm)==null?void 0:h.assets)??k,w="1698852173384",T="sveltekit:snapshot",y="sveltekit:scroll",I="sveltekit:index",f={tap:1,hover:2,viewport:3,eager:4,off:-1};function S(e){let t=e.baseURI;if(!t){const n=e.getElementsByTagName("base");t=n.length?n[0].href:e.URL}return t}function x(){return{x:pageXOffset,y:pageYOffset}}function c(e,t){return e.getAttribute(`data-sveltekit-${t}`)}const d={...f,"":f.hover};function g(e){let t=e.assignedSlot??e.parentNode;return(t==null?void 0:t.nodeType)===11&&(t=t.host),t}function O(e,t){for(;e&&e!==t;){if(e.nodeName.toUpperCase()==="A"&&e.hasAttribute("href"))return e;e=g(e)}}function U(e,t){let n;try{n=new URL(e instanceof SVGAElement?e.href.baseVal:e.href,document.baseURI)}catch{}const a=e instanceof SVGAElement?e.target.baseVal:e.target,r=!n||!!a||A(n,t)||(e.getAttribute("rel")||"").split(/\s+/).includes("external"),l=(n==null?void 0:n.origin)===location.origin&&e.hasAttribute("download");return{url:n,external:r,target:a,download:l}}function L(e){let t=null,n=null,a=null,r=null,l=null,s=null,o=e;for(;o&&o!==document.documentElement;)a===null&&(a=c(o,"preload-code")),r===null&&(r=c(o,"preload-data")),t===null&&(t=c(o,"keepfocus")),n===null&&(n=c(o,"noscroll")),l===null&&(l=c(o,"reload")),s===null&&(s=c(o,"replacestate")),o=g(o);function i(v){switch(v){case"":case"true":return!0;case"off":case"false":return!1;default:return null}}return{preload_code:d[a??"off"],preload_data:d[r??"off"],keep_focus:i(t),noscroll:i(n),reload:i(l),replace_state:i(s)}}function _(e){const t=u(e);let n=!0;function a(){n=!0,t.update(s=>s)}function r(s){n=!1,t.set(s)}function l(s){let o;return t.subscribe(i=>{(o===void 0||n&&i!==o)&&s(o=i)})}return{notify:a,set:r,subscribe:l}}function E(){const{set:e,subscribe:t}=u(!1);let n;async function a(){clearTimeout(n);try{const r=await fetch(`${m}/_app/version.json`,{headers:{pragma:"no-cache","cache-control":"no-cache"}});if(!r.ok)return!1;const s=(await r.json()).version!==w;return s&&(e(!0),clearTimeout(n)),s}catch{return!1}}return{subscribe:t,check:a}}function A(e,t){return e.origin!==location.origin||!e.pathname.startsWith(t)}let b;function N(e){b=e.client}function P(e){return(...t)=>b[e](...t)}const V={url:_({}),page:_({}),navigating:u(null),updated:E()};export{I,f as P,y as S,T as a,U as b,L as c,V as d,k as e,O as f,S as g,N as h,A as i,P as j,x as s};
