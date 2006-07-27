\ ==============================================================================
\
\          chr_test - the test words for the chr module in ffl
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2006-07-27 18:08:01 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/tst.fs
include ffl/chr.fs


.( Testing: chr) cr 

t{ char a chr-lower?   ?true    }t
t{ char A chr-lower?   ?false   }t
t{ char m chr-lower?   ?true    }t
t{ char M chr-lower?   ?false   }t
t{ char z chr-lower?   ?true    }t
t{ char Z chr-lower?   ?false   }t
t{ char { chr-lower?   ?false   }t
t{ char ` chr-lower?   ?false   }t

t{ char A chr-upper?   ?true    }t
t{ char a chr-upper?   ?false   }t
t{ char Q chr-upper?   ?true    }t
t{ char q chr-upper?   ?false   }t
t{ char Z chr-upper?   ?true    }t
t{ char z chr-upper?   ?false   }t
t{ char @ chr-upper?   ?false   }t
t{ char [ chr-upper?   ?false   }t

t{ char A chr-alpha?   ?true    }t
t{ char a chr-alpha?   ?true    }t
t{ char Q chr-alpha?   ?true    }t
t{ char q chr-alpha?   ?true    }t
t{ char Z chr-alpha?   ?true    }t
t{ char z chr-alpha?   ?true    }t
t{ char 0 chr-alpha?   ?false   }t
t{ char @ chr-alpha?   ?false   }t
t{ char [ chr-alpha?   ?false   }t

t{ char 0 chr-digit?   ?true    }t
t{ char 9 chr-digit?   ?true    }t
t{ char 3 chr-digit?   ?true    }t
t{ char a chr-digit?   ?false   }t
t{     bl chr-digit?   ?false   }t
t{ char / chr-digit?   ?false   }t
t{ char : chr-digit?   ?false   }t
  
t{ char A chr-alnum?   ?true    }t
t{ char a chr-alnum?   ?true    }t
t{ char Q chr-alnum?   ?true    }t
t{ char q chr-alnum?   ?true    }t
t{ char Z chr-alnum?   ?true    }t
t{ char z chr-alnum?   ?true    }t
t{ char 0 chr-alnum?   ?true    }t
t{ char @ chr-alnum?   ?false   }t
t{ char [ chr-alnum?   ?false   }t

t{ char A chr-ascii?   ?true    }t
t{ char a chr-ascii?   ?true    }t
t{ char Q chr-ascii?   ?true    }t
t{ char q chr-ascii?   ?true    }t
t{ char Z chr-ascii?   ?true    }t
t{ char z chr-ascii?   ?true    }t
t{ char 0 chr-ascii?   ?true    }t
t{ char @ chr-ascii?   ?true    }t
t{ char [ chr-ascii?   ?true    }t
t{     -1 chr-ascii?   ?false   }t
t{    128 chr-ascii?   ?false   }t
t{   5122 chr-ascii?   ?false   }t

t{     bl chr-blank?   ?true    }t
t{      9 chr-blank?   ?true    }t
t{     11 chr-blank?   ?false   }t
t{ char A chr-blank?   ?false   }t
t{ char a chr-blank?   ?false   }t
t{ char 0 chr-blank?   ?false   }t
t{ char @ chr-blank?   ?false   }t

t{      9 chr-cntrl?   ?true    }t
t{     11 chr-cntrl?   ?true    }t
t{     bl chr-cntrl?   ?false   }t
t{ char a chr-cntrl?   ?false   }t
t{ char 0 chr-cntrl?   ?false   }t

t{ char a chr-graph?   ?true    }t
t{ char A chr-graph?   ?true    }t
t{ char z chr-graph?   ?true    }t
t{ char 0 chr-graph?   ?true    }t
t{ char @ chr-graph?   ?true    }t
t{      9 chr-graph?   ?false   }t
t{     11 chr-graph?   ?false   }t
t{     bl chr-graph?   ?false   }t

t{ char a chr-print?   ?true    }t
t{ char A chr-print?   ?true    }t
t{ char z chr-print?   ?true    }t
t{ char 0 chr-print?   ?true    }t
t{ char @ chr-print?   ?true    }t
t{      9 chr-print?   ?false   }t
t{     11 chr-print?   ?false   }t
t{     bl chr-print?   ?true    }t

t{ char a chr-punct?   ?false   }t
t{ char A chr-punct?   ?false   }t
t{ char z chr-punct?   ?false   }t
t{ char 0 chr-punct?   ?false   }t
t{ char @ chr-punct?   ?true    }t
t{      9 chr-punct?   ?false   }t
t{     11 chr-punct?   ?false   }t
t{     bl chr-punct?   ?false   }t

t{ char a chr-space?   ?false   }t
t{ char A chr-space?   ?false   }t
t{ char z chr-space?   ?false   }t
t{ char 0 chr-space?   ?false   }t
t{ char @ chr-space?   ?false   }t
t{      9 chr-space?   ?true    }t
t{     11 chr-space?   ?true    }t
t{     bl chr-space?   ?true    }t

t{ char a chr-hexdigit?   ?true    }t
t{ char A chr-hexdigit?   ?true    }t
t{ char f chr-hexdigit?   ?true    }t
t{ char D chr-hexdigit?   ?true    }t
t{ char 0 chr-hexdigit?   ?true    }t
t{ char 9 chr-hexdigit?   ?true    }t
t{ char - chr-hexdigit?   ?false   }t
t{     bl chr-hexdigit?   ?false   }t

t{ char 0 chr-octdigit?   ?true    }t
t{ char 7 chr-octdigit?   ?true    }t
t{ char 8 chr-octdigit?   ?false   }t
t{ char a chr-octdigit?   ?false   }t
t{ char @ chr-octdigit?   ?false   }t

t{ s" abc" char d chr-string? ?false }t
t{ s" abc" char a chr-string? ?true  }t
t{ s" abc" char b chr-string? ?true  }t
t{ s" abc" char c chr-string? ?true  }t

t{ char a chr-upper   char A ?s   }t
t{ char A chr-upper   char A ?s   }t
t{ char z chr-upper   char Z ?s   }t
t{ char d chr-upper   char D ?s   }t
t{ char @ chr-upper   char @ ?s   }t
t{     bl chr-upper   bl     ?s   }t

t{ char a chr-lower     char a ?s }t
t{ char A chr-lower     char a ?s }t
t{ char Z chr-lower     char z ?s }t
t{ char Y chr-lower     char y ?s }t
t{ char { chr-lower     char { ?s }t
t{      7 chr-lower     7      ?s }t

\ decimal
t{ char 0 chr-base ?true ?0       }t
t{ char / chr-base ?false         }t
t{ char 9 chr-base ?true 9 ?s     }t
t{ char a chr-base ?false         }t

hex
t{ char 0 chr-base ?true ?0       }t
t{ char / chr-base ?false         }t
t{ char 9 chr-base ?true 9 ?s     }t
t{ char a chr-base ?true A ?s     }t
t{ char A chr-base ?true A ?s     }t
t{ char g chr-base ?false         }t

2 base !
t{ char 0 chr-base ?true ?0       }t
t{ char 1 chr-base ?true 1 ?s     }t
t{ char 2 chr-base ?false         }t

decimal

\ ==============================================================================
