\ ==============================================================================
\
\                 cpx - the complex module in the ffl
\
\               Copyright (C) 2006  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2006-09-11 18:08:21 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] cpx.version [IF]


include ffl/stc.fs


( cpx = Complex module )
( The cpx module implements words for using complex numbers. )

\
\ The code is heavily inspired by the ccmatch library from Daniel A. Atkinson (LGPL) (c) Copyright 2000
\


1 constant cpx.version

  
( Complex structure )

struct: cpx%       ( - n = Get the required space for the cpx data structure )
  float: cpx>re          \ real
  float: cpx>im          \ imaginary
;struct



( Structure creation, initialisation and destruction )

: cpx-init         ( w:cpx - = Initialise the zero complex number )
  dup  0e0 cpx>re f!
       0e0 cpx>im f!
;


: cpx-create       ( "name" - = Create a named complex number in the dictionary )
  create   here   cpx% allot   cpx-init
;


: cpx-new          ( - w:cpx = Create a new complex number on the heap )
  cpx% allocate  throw  dup cpx-init
;


: cpx-free         ( w:cpx - = Free the complex number from the heap )
  free throw 
;


( Calculation module words )

: cpx+add          ( r:re2 r:im2 r:re1 r:im1 - r:re r:im = Add two complex numbers on stack )
  frot f+
  f-rot f+
  fswap
;


: cpx+sub          ( r:re2 r:im2 r:re1 r:im1 - r:re r:im = Subtract complex1 from complex2 on stack )
  frot fswap f-
  f-rot f-
  fswap
;


: cpx+mul          ( r:re2 r:im2 r:re1 r:im1 - r:re r:im = Multiply two complex numbers on stack )
  fswap frot
  f2dup f* f>r                         \ re1 * im2
  f>r
  fswap frot
  f2dup f* fr> fswap f>r               \ re2 * im1
  frot f* f>r f* fr> f-                \ re2 * r1 - im2 * im1
  fr> fr> f+                           \ re1 * im2 + re2 * im1
;


: cpx+rmul         ( r:re r:im r:re2 - r:re r:im = Multiply a complex number with a real number )
  frot
  fover f*                             \ re * re2
  f-rot f*                             \ im * re2
;


: cpx+imul         ( r:re r:im r:im2 - r:re r:im = Multiply a complex number with an imaginary number )
  ftuck f* fnegate                     \ -im * im2 
  f-rot f*                             \  re * im2
;

  
: cpx+div          ( r:re2 r:im2 r:re1 r:im1 - r:re r:im = Divide complex2 by complex1 on stack)
  fover fdup f*
  fover fdup f*
  f+ f>r                               \ r = re1 * re1 + im1 * im1
  frot
  f2dup f*                             \ im1 * im2
  f>r f>r
  fswap frot
  f2dup f*                             \ re1 * re2
  fr> fswap fr> f+                     \ re1 * re2 + im1 * im2
  fr@ f/ f>r                           \ re1 * re2 + im1 * im2 / r
  frot f*                              \ im2 * re1
  fswap frot f*                        \ re2 * im1
  f-                                   \ re1 * im2 - re2 * im1
  fr> fswap fr> f/                     \ re1 * im2 - re2 * im1 / r
;


: cpx+conj         ( r:re r:im - r:re r:im = Conjugate the complex number on stack )
  fnegate                              \ negate the imaginary part
;


: cpx+nrm          ( r:re r:im - r = Calculate the square of the modulus of the complex number )
  fdup f*
  fswap fdup f*
  f+                                   \ re * re + im * im
;


: cpx+abs          ( r:re r:im - r = Calculate the modulus of the complex number )
  cpx+nrm fsqrt                        \  sqrt(re * re + im * im)
;


: cpx+sqrt         ( r:re r:im - r:re r:im = Calculate the square root for the complex number on stack )
  fswap
  f2dup fswap cpx+abs                  \ r = abs(re,im)
  fover fabs f+ 2e0 f/ fsqrt           \ r = sqrt(0.5*(r+abs(re)))
  fdup f0= IF                          \ if r = 0 then
    fdrop fdrop fdrop                  \   re = 0
    0e0 0e0                            \   im = 0
  ELSE
    fswap f0< IF                       \ else if re < 0
      fover fabs fover f/ 2e0 f/       \   re = 0.5*(fabs(im)/r)
      frot f0< IF                      \   if im < 0
        fswap                          \     im = -r
        fnegate
      ELSE                             \   else
        fswap                          \     im = r
      THEN
    ELSE                               \ else if re >= 0
      fswap                            \   re = r
      fover f/ 2e0 f/                  \   im = 0.5*(im/r)
    THEN
  THEN
;


: cpx+exp          ( r:re r:im - r:re r:im = Calculate the exponent function for the complex number on stack )
  fsincos                              \ sin(im) cos(im)
  frot fexp                            \ exp(re)
  ftuck f*                             \ exp(re) * cos(im)
  f-rot f*                             \ exp(re) * sin(im)
;


: cpx+ln           ( r:re r:im - r:re r:im = Calculate the natural logarithm for the complex number on stack )
  f2dup cpx+nrm                        \ r = nrm
  fln 2e0 f/                           \ im = 0.5*ln(r)
  f-rot fswap fatan2                   \ re = atan2(im,re)
;


: cpx+sin          ( r:re r:im - r:re r:im = Calculate the trigonometric functions sine for the complex number on stack )
  fexp fswap fsincos                   \ u = exp(im) sin(re) cos(re)
  frot fdup 1e0 fswap f/               \ v = 1 / u
  ftuck f+ 2e0 f/                      \ u = 1/2 * (u+v)
  fdup frot f-                         \ v = u - v
  frot f*                              \ im = v * cos(re)
  f-rot f* fswap                       \ re = u * sin(re)
;


: cpx+cos          ( r:re r:im - r:re r:im = Calculate the trigonometric functions cosine for the complex number on stack)
  fexp fswap fsincos                   \ u = exp(im) sin(re) cos(re)
  frot fdup 1e0 fswap f/               \ v = 1 / u
  ftuck f+ 2e0 f/                      \ u = 1/2 * (u+v)
  fdup frot f-                         \ v = u - v
  f>r f*                               \ re = u * cos(re)
  fswap fr> f* fnegate                 \ im = -v * sin(re)
;


: cpx+tan          ( r:re r:im - r:re r:im = Calculate the trigonometric functions trangent for the complex number on stack )
  fexp fswap fsincos                   \ u = exp(im) sin(re) cos(re) 
  frot fdup 1e0 fswap f/               \ v = 1/u
  ftuck f+ 2e0 f/                      \ u = (u+v)/2
  fdup frot f-                         \ v = u - v
  frot fswap
  fover fdup f*                        \ c * c
  fover fdup f*                        \ v * v
  f+ f>r                               \ d = c * c + v * v
  frot f* fr@ f/                       \ im = (u * v) / d
  f-rot f* fr> f/                      \ re = (s * c) / d
  fswap
;

  
: cpx+asin         ( r:re r:im - r:re r:im = Calculate the inverse trigonometric function sine for the complex number on stack )
  f2dup
  f2dup cpx+mul                        \ w = re,im * re,im
  1e0 f-rot 
  0e0 f-rot cpx+sub                    \ u = 1,0 - w
  cpx+sqrt                             \ u = sqrt(u)
  fswap frot f-                        \ v.re = u.re - im
  f-rot f+                             \ v.im = u.im + re
  cpx+ln                               \ y = ln(v)
  fswap fnegate                        \ re = y.im  im = -y.re
;

  
: cpx+acos         ( r:re r:im - r:re r:im = Calculate the inverse trigonometric function cosine for the complex number on stack)
  f2dup
  f2dup cpx+mul                        \ w = re,im * re,im
  1e0 f-rot 
  0e0 f-rot cpx+sub                    \ u = 1,0 - w
  cpx+sqrt                             \ u = sqrt(u)
  fswap frot f+                        \ re = re - v.im
  f-rot f-                             \ im = im + v.re
  fswap
  cpx+ln                               \ y = ln(re,im)
  fswap fnegate                        \ re = y.im  im = -y.re
;


: cpx+atan         ( r:re r:im - r:re r:im = Calculate the inverse trigonometric function tangent for the complex number on stack )
  fnegate fswap                        \ u = -im,re
  f>r f>r 1e0 0e0 fr> fr>
  f2dup f>r f>r
  cpx+add                              \ s = 1,0 + u
  1e0 0e0 fr> fr> 
  cpx+sub                              \ w = 1,0 - u
  cpx+div                              \ s = s \ w
  cpx+ln                               \ s = ln(s)
  -0.5e0 cpx+imul                      \ s= -0.5j * s
;


: cpx+sinh         ( r:re r:im - r:re r:im = Calculate the hyperbolic function sine for the complex number on stack )
  fsincos                              \ sin(im) cos(im)
  frot fexp                            \ u = exp(re)
  1e0 fover f/                         \ v = 1/u
  ftuck f+ 2e0 f/                      \ u = 0.5*(u+v)
  fdup frot f-                         \ v = u-v                
  frot f*                              \ re = cos(im) * v
  f-rot f*                             \ im = sin(im) * u
;


: cpx+cosh         ( r:re r:im - r:re r:im = Calculate the hyperbolic function cosine for the complex number on stack )
  fsincos                              \ sin(im) cos(im)
  frot fexp                            \ u = exp(re)
  1e0 fover f/                         \ v = 1/u
  ftuck f+ 2e0 f/                      \ u = 0.5*(u+v)
  fdup frot f-                         \ v = u-v
  fswap frot f*                        \ re = cos(im) * u
  f-rot f*                             \ im = sin(im) * v
;


: cpx+tanh         ( r:re r:im - r:re r:im = Calculate the hyperbolic function tangent for the complex number on stack )
  fsincos                              \ s = sin(im) c = cos(im)
  frot fexp                            \ u = exp(re)
  1e0 fover f/                         \ v = 1/u
  ftuck f+ 2e0 f/                      \ u = 0.5*(u+v)
  fdup frot f-                         \ v = u-v
  frot fover fdup f*
  fover fdup f* f+ f>r                 \ d = c*c+v*v
  fswap frot f* fr@ f/                 \ re = (u*v)/d
  f-rot f* fr> f/                      \ im = (s*c)/d
;


: cpx+asinh        ( r:re r:im - r:re r:im = Calculate the inverse hyperbolic function sine for the complex number on stack )
  f2dup                                \ w = (re,im)
  f2dup cpx+mul                        \ w = w * w
  1e0 0e0 cpx+add                      \ u = (1,0) + w
  cpx+sqrt                             \ u = sqrt(u)
  cpx+add                              \ (re,im) = (re,im) + u
  cpx+ln                               \ (re,im) = ln(re,im)
;


: cpx+acosh        ( r:re r:im - r:re r:im = Calculate the inverse hyperbolic function cosine for the complex number on stack )
  f2dup
  f0= -1e0 f< AND                      \ f = (im = 0) AND (re < -1)
  f2dup                                \ w = (re,im)
  f2dup cpx+mul                        \ w = w * w
  1e0 0e0 cpx+sub                      \ w = w - (1,0)
  cpx+sqrt                             \ w = sqrt(w)
  cpx+add                              \ w = (re,im) + w
  cpx+ln                               \ w = ln(w)
  fover f0< IF                         \ if (w.re < 0.0)
    fnegate fswap                      \   w.re = -w.re
    fnegate fswap                      \   w.im = -w.im
  THEN
  IF                                   \ if (f)
    fnegate                            \   w.im = -w.im
  THEN
;


: cpx+atanh        ( r:re r:im - r:re r:im = Calculate the inverse hyperbolic function tangent for the complex number on stack )
  f2dup f>r f>r
  1e0 0e0 cpx+add                      \ u = (1,0) + (re,im)
  1e0 0e0 fr> fr> cpx+sub              \ w = (1,0) - (re,im)
  cpx+div                              \ u = u / w
  cpx+ln                               \ u = ln(u)
  0.5e0 cpx+rmul                       \ re,im = 0.5 * u
;


( Private words )

: cpx+convert      ( r c-addr - c-addr f = Convert a float number to a string )
  [char] 0 over c! char+
  [char] . over c! char+               \ ToDo: locale
  precision 1 max 32 min               \ Limit precision: PAD is at least 84 characters
  2dup
  represent
  IF
    2>r chars + 2r>                    \ success: update address with precision
    rot [char] e over c! char+         \ add exponent indication
    rot s>d tuck dabs <# #s rot sign #> 
    rot swap 2dup chars +              \ add exponent
    >r cmove r>
    swap
  ELSE
    2drop drop                         \ no success: clear the stack ToDo: exception ??
    false
  THEN
;

  
( Conversion module words )

: cpx+to-string    ( r:re r:im - c-addr u = Convert complex number to a string, using precision and PAD )
  fswap                                \ re is converted first
  pad
  bl over c! dup char+                 \ start of string, reserve space for sign
  cpx+convert IF
    swap [char] - over c! swap         \ negative: set sign
  ELSE
    swap char+ swap                    \ positive: move start after sign
  THEN
  [char] + over c! dup char+           \ put + for positive sign
  cpx+convert IF
    swap [char] - swap c!
  ELSE
    nip
  THEN
  [char] j over c! char+
  over - 1 chars /                     \ calculate number of chars
;


: cpx+to-polar     ( r:re r:im - r:r r:theta = Convert complex number to polar )
  f2dup cpx+abs                       \ r     = abs(re,im)
  f-rot fswap fatan2                  \ theta = atan2(im,re) ToDo: problems ??
;


: cpx+from-polar   ( r:r r:theta - r:re r:im = Convert polar to complex number )
  fsincos frot cpx+rmul fswap         \ re = cos * r im = sin * r
;


( Compare module words )

: cpx+equal?       ( r:re2 r:im2 r:re1 r:im1 - f = Check if two complex numbers are [true] equal )
  frot f= f= AND
;


( Structure words )

: cpx-re@         ( w:cpx - r:re = Get the real part of the complex number )
  cpx>re f@
;


: cpx-im@       ( w:cpx - r:im = Get the imaginary part of the complex number )
  cpx>im f@
;


: cpx-get          ( w:cpx - r:re r:im = Get the complex number )
  >r
  r@ cpx-re@ 
  r> cpx-im@
;

  
: cpx-set          ( r:re r:im w:cpx = Set the complex number )
  >r
  r@ cpx>im f!
  r> cpx>re f!
;


: cpx^move         ( w:cpx2 w:cpx1 - = Move complex2 in complex1 )
  swap cpx-get
  cpx-set
;


: cpx^equal?       ( w:cpx2 w:cpx1 - f = Check if complex2 is [true] equal to complex1 )
  >r cpx-get 
  r> cpx-get
  cpx+equal?
;


: cpx-dump         ( w:cpx - = Dump the complex )
  ." cpx:" dup . cr
  ."   re:" dup cpx>re  f@ f. cr
  ."   im:"     cpx>im  f@ f. cr
;

[THEN]

\ ==============================================================================
