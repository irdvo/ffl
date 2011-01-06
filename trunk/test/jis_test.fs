\ ==============================================================================
\
\        jis_test - the test words for the jis module in the ffl
\
\               Copyright (C) 2010  Dick van Oudheusden
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
\  $Date: 2008-03-02 15:03:03 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/jis.fs
include ffl/tst.fs
include ffl/est.fs


.( Testing: jis) cr

t{ jis-create jis1 }t

\ ToDo: \n
t{ s\" {\"value\":10,\"flag\":false,\"array\":[1,2,3.1],\"string\":\"hello\",\"empty\":\"\"}" jis1 jis-set-string }t

t{ jis1 jis-read  jis.start-object    ?s                 }t
t{ jis1 jis-read  jis.name            ?s s" value" ?str  }t
t{ jis1 jis-read  jis.number          ?s 10 ?s           }t
t{ jis1 jis-read  jis.name            ?s s" flag"  ?str  }t
t{ jis1 jis-read  jis.boolean         ?s ?false          }t
t{ jis1 jis-read  jis.name            ?s s" array" ?str  }t
t{ jis1 jis-read  jis.start-array     ?s                 }t
t{ jis1 jis-read  jis.number          ?s 1 ?s            }t
t{ jis1 jis-read  jis.number          ?s 2 ?s            }t
t{ jis1 jis-read  jis.float           ?s 3.1E0 ?r        }t
t{ jis1 jis-read  jis.end-array       ?s                 }t
t{ jis1 jis-read  jis.name            ?s s" string" ?str }t
t{ jis1 jis-read  jis.string          ?s s" hello" ?str  }t
t{ jis1 jis-read  jis.name            ?s s" empty" ?str  }t
t{ jis1 jis-read  jis.string          ?s s" "  ?str      }t
t{ jis1 jis-read  jis.end-object      ?s                 }t
t{ jis1 jis-read  jis.done            ?s                 }t

t{ jis-new value jis2 }t

t{ s\" { \"first\"\n:\t{\"second\"\r: {\"third\":{\"special\\/\":\"\\\"\\n\\b\\f\\r\\\\\\t\"},\"value\":2},\"value\":3}}" jis2 jis-set-string }t

t{ jis2 jis-read jis.start-object  ?s                    }t
t{ jis2 jis-read jis.name          ?s s" first" ?str     }t
t{ jis2 jis-read jis.start-object  ?s                    }t
t{ jis2 jis-read jis.name          ?s s" second" ?str    }t
t{ jis2 jis-read jis.start-object  ?s                    }t
t{ jis2 jis-read jis.name          ?s s" third" ?str     }t
t{ jis2 jis-read jis.start-object  ?s                    }t
t{ jis2 jis-read jis.name          ?s s\" special/" ?str }t
t{ jis2 jis-read jis.string  ?s s\" \"\n\b\f\r\\\t" ?str }t
t{ jis2 jis-read jis.end-object    ?s                    }t
t{ jis2 jis-read jis.name          ?s s" value" ?str     }t
t{ jis2 jis-read jis.number        ?s 2 ?s               }t
t{ jis2 jis-read jis.end-object    ?s                    }t
t{ jis2 jis-read jis.name          ?s s" value" ?str     }t
t{ jis2 jis-read jis.number        ?s 3 ?s               }t
t{ jis2 jis-read jis.end-object    ?s                    }t
t{ jis2 jis-read jis.end-object    ?s                    }t
t{ jis2 jis-read jis.done          ?s                    }t


variable jis-cb-state  jis-cb-state on

: jis-cb  ( addr -- c-addr u | 0 )
  dup @ IF
    off
    s\" [{\"first\":1},[],{\"second\":2}]"
  ELSE
    drop 0
  THEN
;

t{ jis-cb-state ' jis-cb jis2 jis-set-reader }t

t{ jis2 jis-read jis.start-array   ?s                      }t
t{ jis2 jis-read jis.start-object  ?s                      }t
t{ jis2 jis-read jis.name          ?s s" first" ?str       }t
t{ jis2 jis-read jis.number        ?s 1 ?s                 }t
t{ jis2 jis-read jis.end-object    ?s                      }t
t{ jis2 jis-read jis.start-array   ?s                      }t
t{ jis2 jis-read jis.end-array     ?s                      }t
t{ jis2 jis-read jis.start-object  ?s                      }t
t{ jis2 jis-read jis.name          ?s s" second" ?str      }t
t{ jis2 jis-read jis.number        ?s 2 ?s                 }t
t{ jis2 jis-read jis.end-object    ?s                      }t
t{ jis2 jis-read jis.end-array     ?s                      }t
t{ jis2 jis-read jis.done          ?s                      }t

t{ jis2 jis-free }t

[THEN]

\ ==============================================================================
