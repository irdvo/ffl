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
\  $Date: 2006-09-02 14:56:21 $ $Revision: 1.3 $
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
  dup    cpx>re 0!
         cpx>im 0!
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
  frot frot f+
  fswap
;


: cpx+sub          ( r:re2 r:im2 r:re1 r:im1 - r:re r:im = Subtract complex1 from complex2 on stack )
  frot fswap f-
  frot frot f-
  fswap
;


: cpx+mul          ( r:re2 r:im2 r:re1 r:im1 - r:re r:im = Multiply two complex numbers on stack )
  fswap frot
  fover fover f* f>r                   \ re1 * im2
  f>r
  fswap frot
  fover fover f* fr> fswap f>r         \ re2 * im1
  frot f* f>r f* fr> f-                \ re2 * r1 - im2 * im1
  fr> fr> f+                           \ re1 * im2 + re2 * im1
;


: cpx+rmul         ( r:re r:im r:re2 - r:re r:im = Multiply a complex number with a real number )
  frot
  fover f*                             \ re * re2
  frot frot f*                         \ im * re2
;


: cpx+imul         ( r:re r:im r:im2 - r:re r:im = Multiply a complex number with an imaginary number )
  fswap fover f* fnegate               \ -im * im2 
  frot frot f*                         \  re * im2
;

  
: cpx+div          ( r:re2 r:im2 r:re1 r:im1 - r:re r:im = Divide complex2 by complex1 on stack)
  fover fdup f*
  fover fdup f*
  f+ f>r                               \ r = re1 * re1 + im1 * im1
  frot
  fover fover f*                       \ im1 * im2
  f>r f>r
  fswap frot
  fover fover f*                       \ re1 * re2
  fr> fswap fr> f+                     \ re1 * re2 + im1 * im2
  fr@ f/ f>r                           \ re1 * re2 + im1 * im2 / r
  frot f*                              \ im2 * re1
  fswap frot f*                        \ re2 * im1
  f-                                   \ re1 * im2 - re2 * im1
  fr> fswap fr> f/                     \ re1 * im2 - re2 * im1 / r
;


: cpx+conj         ( r:re r:im - r:re r:im = Conjugate the complex number on stack )
  negate                     \ negate the imaginary part
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
\  double r = [self abs];

\  r = sqrt(ldexp(r + fabs(_re), -1));
\  if (r == 0.0) 
\  {
\    _re = 0.0;
\    _im = 0.0;
\  }
\  else
\  {
\    if (_re >= 0.0)
\    { 
\      _re = r; 
\      _im = ldexp(_im / r, -1);
\    }
\    else
\    { 
\      _re = ldexp(fabs(_im) / r, -1);
\      if (_im >= 0.0) 
\      {
\        _im = r; 
\      }
\      else
\      {
\        _im= -r;
\      }
\    }
\  }
;


: cpx+exp          ( r:re r:im - r:re r:im = Calculate the exponent function for the complex number on stack )
  fsincos                              \ sin(im) cos(im)
  frot fexp                            \ exp(re)
  fswap fover f*                       \ exp(re) * cos(im)
  frot frot f*                         \ exp(re) * sin(im)
;


: cpx+ln           ( r:re r:im - r:re r:im = Calculate the natural logarithm for the complex number on stack )
\  double r = [self nrm];

\  _im = atan2(_im, _re);
\  _re = ldexp(log(r), -1);
;


: cpx+sin          ( r:re r:im - r:re r:im = Calculate the trigonometric functions sine for the complex number on stack )
  fexp fswap fsincos                   \ u = exp(im) sin(re) cos(re)
  frot fdup 1e0 fswap f/               \ v = 1 / u
  fswap fover f+ 2e0 f/                \ u = 1/2 * (u+v)
  fdup frot f-                         \ v = u - v
  frot f*                              \ im = v * cos(re)
  frot frot f* fswap                   \ re = u * sin(re)
;


: cpx+cos          ( r:re r:im - r:re r:im = Calculate the trigonometric functions cosine for the complex number on stack)
  fexp fswap fsincos                   \ u = exp(im) sin(re) cos(re)
  frot fdup 1e0 fswap f/               \ v = 1 / u
  fswap fover f+ 2e0 f/                \ u = 1/2 * (u+v)
  fdup frot f-                         \ v = u - v
  f>r f*                               \ re = u * cos(re)
  fswap fr> f* fnegate                 \ im = -v * sin(re)
;


: cpx+tan          ( r:re r:im - r:re r:im = Calculate the trigonometric functions trangent for the complex number on stack )
  fexp fswap fsincos                   \ u = exp(im) sin(re) cos(re) 
  frot fdup 1e0 fswap f/               \ v = 1/u
  fswap fover f+ 2e0 f/                \ u = (u+v)/2
  fdup frot f-                         \ v = u - v
  frot fswap
  fover fdup f*                        \ c * c
  fover fdup f*                        \ v * v
  f+ f>r                               \ d = c * c + v * v
  frot f* fr@ f/                       \ im = (u * v) / d
  frot frot f* fr> f/                  \ re = (s * c) / d
  fswap
;

  
: cpx+asin         ( r:re r:im - r:re r:im = Calculate the inverse trigonometric function sine for the complex number on stack )
\  DComplex *u = [DComplex alloc]; 
\  DComplex *w = [self copy];
  
\  double    t;

\  [u init :1.0 :0.0];
\  [w move :self];
  
\  [w mul :w];
\  [u sub :w];
\  [u sqrt  ];

\  t   = _re;  
\  _re = [u re] - _im; 
\  _im = [u im] +  t;
  
\  [self log];

\  t   = _re;
\  _re = _im; 
\  _im = -t;

\  [u free];
\  [w free];
;


: cpx+acos         ( r:re r:im - r:re r:im = Calculate the inverse trigonometric function cosine for the complex number on stack)
\  DComplex *u = [DComplex alloc];
\  DComplex *w = [self copy];
\  double    t;

\  [u init :1.0 :0.0];
  
\  [w mul :w];
\  [u sub :w];
\  [u sqrt  ];
 
\  _re -= [u im]; 
\  _im += [u re]; 

\  [self log];

\   t  = _re;
\  _re = _im; 
\  _im =  t;
;


: cpx+atan         ( r:re r:im - r:re r:im = Calculate the inverse trigonometric function tangent for the complex number on stack )
\  DComplex *u = [DComplex alloc];
\  DComplex *w = nil;

\  [u init :-_im :_re];
  
\  _re = 1.0;
\  _im = 0.0;

\  w = [self copy];

\  [self add :u];
\  [w    sub :u];
\  [self div :w];

\  [self log];
\  [self imul :-0.5];
;


: cpx+sinh         ( r:re r:im - r:re r:im = Calculate the hyperbolic function sine for the complex number on stack )
\  double s = sin(_im);
\  double c = cos(_im);
\  double u = exp(_re);
\  double v = 1.0 / u;

\  u = ldexp(u + v, -1);
\  v = u - v;

\  _re = v * c;
\  _im = u * s;
;


: cpx+cosh         ( r:re r:im - r:re r:im = Calculate the hyperbolic function cosine for the complex number on stack )
\  double s = sin(_im);
\  double c = cos(_im);
\  double u = exp(_re);
\  double v = 1.0 / u;

\  u = ldexp(u + v, -1);
\  v = u - v;

\  _re = c * u;
\  _im = v * s;
;


: cpx+tanh         ( r:re r:im - r:re r:im = Calculate the hyperbolic function tangent for the complex number on stack )
\  double s = sin(_im);
\  double c = cos(_im);
\  double u = exp(_re); 
\  double v = 1.0 / u;
\  double d;

\  u = ldexp(u + v, -1); 
\  v = u - v;
\  d = c * c + v * v; 

\  _re = u * v / d;
\  _im = s * c / d;
;


: cpx+asinh        ( r:re r:im - r:re r:im = Calculate the inverse hyperbolic function sine for the complex number on stack )
\  DComplex *u = [DComplex alloc];
\  DComplex *w = [self copy];

\  [u init :1.0 :0.0];
  
\  [w mul :w];
\  [u add :w];
\  [u sqrt  ];

\  [self add :u];
\  [self log   ];

\  [u free];
\  [w free];
;


: cpx+acosh        ( r:re r:im - r:re r:im = Calculate the inverse hyperbolic function cosine for the complex number on stack )
\  DComplex *u = [DComplex alloc];
\  DComplex *w = [self copy];
\  int      kf = ((_im == 0) && (_re < -1)) ? 1 : 0;

\  [u init :1.0 :0.0];
  
\  [w mul  :w];
\  [w sub  :u];
\  [w sqrt   ];

\  [u move :w];

\  [self add :u];
\  [self log   ]; 

\  if (_re < 0.0)
\  {
\    _re= -_re;
\    _im= -_im;
\  }
\  if (kf) 
\    _im= -_im;

\  [u free];
\  [w free];
;


: cpx+atanh        ( r:re r:im - r:re r:im = Calculate the inverse hyperbolic function tangent for the complex number on stack )
\  DComplex *u = [DComplex alloc]; 
\  DComplex *w = [DComplex alloc]; 

\  [[w init :1.0 :0.0] sub :self];
\  [[u init :1.0 :0.0] add :self];

\  [u div :w   ];
\  [u log      ];
\  [u rmul :0.5];

\  [self move :u];

\  [u free];
\  [w free];
;


( Conversion module words )

: cpx+to-string    ( r:re r:im - c-addr u = Convert complex number to a string )
\  DText *str = [[DText alloc] init];

\  if (_re == 0.0)
\  {
\    [str format :"%gj",_im];
\  }
\  else if (_im == 0.0)
\  {
\    [str format :"%g", _re];
\  }
\  else if (_im < 0.0)
\  {
\    [str format :"%g%gj",_re,_im];
\  }
\  else
\  {
\    [str format :"%g+%gj",_re,_im];
\  }
;


: cpx+to-polar     ( r:re r:im - r:r r:theta = Convert complex number to polar )
  fover fover cpx+abs                 \ r     = abs(re,im)
  frot frot fswap fatan2              \ theta = atan2(im,re) ToDo: problems ??
;


: cpx+from-polar   ( r:r r:theta - r:re r:im = Convert polar to complex number )
  fsincos frot cpx+rmul fswap         \ re = cos * r im = sin * r
;


( Compare module words )

: cpx+equal?       ( r:re2 r:im2 r:re1 r:im1 - f = Check if two complex numbers are equal )
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
;


: cpx^equal?       ( w:cpx2 w:cpx1 - f = Check if complex2 is equal to complex1 )
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
