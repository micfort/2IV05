__author__="miblon"
__date__ ="$Jun 13, 2011 11:34:17 AM$"

from time import *
import sys, traceback

# An extremely simple Singleton logger
class Log:
    # Singleton: sole static instance of Log to have a single Log object
    log = None

    # timer to keep start of time interval
    # TODO: make this stack to support nested timings
    t1 = None

    def __init__(self, args):
        self.args = args
        # Singleton: sole instance of Log o have a single Log object
        Log.log = self

    def pr(self, message):
        print (message)
        sys.stdout.flush()
        return message

    def debug(self, message):
        if self.args.verbose:
            return Log.log.pr("DEBUG: " + message)

    def info(self, message):
        return Log.log.pr("INFO: " + message)

    def warn(self, message):
        return Log.log.pr("WARN: " + message)

    def error(self, message):
        if self.args.verbose:
            return Log.log.pr("ERROR: " + message + ' ' + Log.log.get_exception_info())
        else:
            return Log.log.pr("ERROR: " + message)

    def fatal(self, message):
        if self.args.verbose:
            return Log.log.pr("FATAL: " + message + ' ' + Log.log.get_exception_info())
        else:
            return Log.log.pr("FATAL: " + message)
        sys.exit(-1)

    def time(self, message=""):
        return self.info(message + " " + strftime("%Y-%m-%d %H:%M:%S", localtime()))

    # Start (global) + print timer: useful to time for processing and optimization
    def startTimer(self, message=""):
        Log.t1 = time()
        return self.info("START: " + message)

    # End (global) timer + print seconds passed: useful to time for processing and optimization
    def endTimer(self, message=""):
        return self.info("END: " + message + " tijd=" + str( round( (time() - Log.t1) , 0)) + " sec")

    # Maak gedetailleerde exceptie stack info
    def get_exception_info(self):
        exc_type, exc_value, exc_traceback = sys.exc_info()
        return repr(traceback.format_exception(exc_type, exc_value,exc_traceback))

