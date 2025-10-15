import { HttpInterceptorFn, HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { environment } from '../../environments/environment';

// Prefija apiBaseUrl cuando la URL es relativa (comienza con '/api' o similar)
export const apiBaseInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  let request = req;

  const isAbsolute = /^https?:\/\//i.test(req.url);
  const isApiRelative = req.url.startsWith('/api') || req.url.startsWith('api/');

  if (!isAbsolute && isApiRelative) {
    const normalized = req.url.startsWith('/') ? req.url.substring(1) : req.url;
    const base = environment.apiBaseUrl.replace(/\/$/, '');
    // Evitar doble /api si alguien configuró base con /api
    const baseNoApi = base.endsWith('/api') ? base.slice(0, -4) : base;
    request = req.clone({ url: `${baseNoApi}/${normalized}` });
  }

  return next(request);
};
